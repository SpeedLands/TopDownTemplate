using System.Collections;
using System.Text; // Importante para StringBuilder
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private enum QuestState { NotStarted, InProgress, Completed }
    private QuestState questState = QuestState.NotStarted;

    public void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        // Esta validación está bien
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
        {
            return;
        }

        // --- LÓGICA CORREGIDA ---
        if (!isDialogueActive)
        {
            // Si no hay un diálogo activo, lo empezamos
            StartDialog();
        }
        else
        {
            // Si ya hay un diálogo, avanzamos a la siguiente línea
            NextLine();
        }
    }

    // CORRECCIÓN: Renombrado de StratDialog a StartDialog
    void StartDialog()
    {
        SyncQuestState();

        if (questState == QuestState.NotStarted)
        {
            dialogueIndex = 0;
        }
        else if (questState == QuestState.InProgress)
        {
            dialogueIndex = dialogueData.questInProgressIndex;
        }
        else if (questState == QuestState.Completed)
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }

        isDialogueActive = true;

        dialogueUI.setNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);
        DisplayCurrentline();
    }

    private void SyncQuestState()
    {
        if (dialogueData.quest == null) return;

        string questID = dialogueData.quest.questID;

        if (QuestController.Instance.IsQuestCompleted(questID) || QuestController.Instance.IsQuestHandedIn(questID))
        {
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }
    }

    void NextLine()
    {
        if (isTyping)
        {
            // Si está escribiendo, termina la línea de golpe
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        dialogueUI.ClearChoices();

        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
        }

        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            // Si no está escribiendo, pasa a la siguiente línea
            DisplayCurrentline();
        }
        else
        {
            // Si era la última línea, termina el diálogo
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        // MEJORA: Usar StringBuilder para eficiencia
        StringBuilder stringBuilder = new StringBuilder();
        dialogueUI.SetDialogueText(""); // Limpiar el texto antes de empezar

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter); // Usar SetText es mejor para TMP
            SoundEffectManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // La lógica de auto-avance está bien
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int newIndex = choice.nextDialogueIndexes[i];
            bool givesQuest = choice.givesQuest[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChoseOption(newIndex, givesQuest));
        }
    }

    void ChoseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentline();
    }

    void DisplayCurrentline()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        if (questState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questID))
        {
            HandleQuestCompletion(dialogueData.quest);
        }
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }

    void HandleQuestCompletion(Quest quest)
    {
        QuestController.Instance.HandInQuest(quest.questID);
    }
}