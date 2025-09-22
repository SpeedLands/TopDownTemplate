using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }
    public List<QuestProgress> activeQuests = new();
    private QuestUI questUI;
    public List<string> handingQuestIDs = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        questUI = FindAnyObjectByType<QuestUI>();
        InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuest;
    }

    public void AcceptQuest(Quest quest)
{
    // AÑADE ESTA LÍNEA
    Debug.Log($"Intentando aceptar la misión: '{quest.questName}' con ID: {quest.questID}");

    if (IsQuestActive(quest.questID)) return;
    activeQuests.Add(new QuestProgress(quest));
    CheckInventoryForQuest();
    questUI.UpdateQuestUI();
}

    public bool IsQuestActive(string questID) => activeQuests.Exists(q => q.QuestId == questID);

    public void CheckInventoryForQuest()
{
    
    Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

    foreach (QuestProgress quest in activeQuests)
    {

        foreach (QuestObjective questObjective in quest.quest.objectives)
        {

            if (questObjective.type != ObjectiveType.CollectItem)
            {
                continue;
            }
            
            if (!int.TryParse(questObjective.objectiveID, out int itemID))
                {
                    continue;
                }
            int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0;
            
            if (questObjective.currentAmount != newAmount)
            {
                questObjective.currentAmount = newAmount;
            }
        }
    }
    questUI.UpdateQuestUI();
}

    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activeQuests.Find(q => q.QuestId == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInQuest(string questID)
    {
        if (!RemoveRequieredItemsFromInventory(questID))
        {
            return;
        }

        QuestProgress quest = activeQuests.Find(q => q.QuestId == questID);
        if (quest != null)
        {
            handingQuestIDs.Add(questID);
            activeQuests.Remove(quest);
            questUI.UpdateQuestUI();
        }
    }

    public bool IsQuestHandedIn(string questID)
    {
        return handingQuestIDs.Contains(questID);
    }
    public bool RemoveRequieredItemsFromInventory(string questID)
    {
        QuestProgress quest = activeQuests.Find(q => q.QuestId == questID);
        if (quest == null) return false;

        Dictionary<int, int> requiredItems = new();
        foreach (QuestObjective objective in quest.quest.objectives)
        {
            if (objective.type == ObjectiveType.CollectItem && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItems[itemID] = objective.requiredAmount;

            }
        }

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (var item in requiredItems)
        {
            if (itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                return false;
            }
        }

        foreach (var itemRequirement in requiredItems)
        {
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
        }

        return true;
    }

    public void LoadQuestProgress(List<QuestProgress> savedQuests)
    {
        activeQuests = savedQuests ?? new();
        CheckInventoryForQuest();
        questUI.UpdateQuestUI();
    }
}
