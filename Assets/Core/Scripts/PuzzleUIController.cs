using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PuzzleUIController : MonoBehaviour
{
    public static PuzzleUIController Instance { get; private set; }

    [Header("UI Panels & Areas")]
    [SerializeField] private GameObject puzzleScreen;
    [SerializeField] private Transform commandPalette;
    [SerializeField] private Transform executionArea;
    [SerializeField] private Button executeButton;
    [SerializeField] private Button closeButton; // Botón para cerrar

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        executeButton.onClick.AddListener(OnExecuteButtonPressed);
        // --- LÓGICA AÑADIDA ---
        closeButton.onClick.AddListener(OnCloseButtonPressed);
    }

    // --- NUEVO MÉTODO ---
    private void OnCloseButtonPressed()
    {
        // El PuzzleManager se encarga de la lógica de descarga.
        PuzzleManager.Instance.UnloadPuzzle();
    }

    public void ShowPuzzleScreen(bool show, LevelData levelData = null)
    {
        puzzleScreen.SetActive(show);
        if (show && levelData != null)
        {
            PopulateCommandPalette(levelData.allowedCommands);
        }
        else
        {
            ClearCommandPalette();
            ClearExecutionArea();
        }
    }

    private void PopulateCommandPalette(List<GameObject> allowedCommands)
    {
        ClearCommandPalette();
        foreach (GameObject commandPrefab in allowedCommands)
        {
            Instantiate(commandPrefab, commandPalette);
        }
    }

    private void ClearCommandPalette()
    {
        foreach (Transform child in commandPalette) Destroy(child.gameObject);
    }

    private void ClearExecutionArea()
    {
        foreach (Transform child in executionArea) Destroy(child.gameObject);
    }

    public void OnExecuteButtonPressed()
    {
        List<Command> commandSequence = new List<Command>();
        foreach (Transform child in executionArea)
        {
            CommandUIMetadata metadata = child.GetComponent<CommandUIMetadata>();
            if (metadata != null)
            {
                Command command = CreateCommandFromType(metadata.commandType);
                if (command != null) commandSequence.Add(command);
            }
        }

        if (commandSequence.Count > 0)
        {
            PuzzleManager.Instance.ExecuteCommandSequence(commandSequence);
        }
    }

    private Command CreateCommandFromType(CommandType type)
    {
        switch (type)
        {
            case CommandType.MoveForward: return new MoveForwardCommand();
            case CommandType.TurnRight:   return new TurnRightCommand();
            case CommandType.TurnLeft:    return new TurnLeftCommand();
            case CommandType.ToggleSpikes: return new ToggleSpikesCommand();
            default: return null;
        }
    }

    // --- El resto de la lógica de arrastrar y soltar no cambia ---
    public void HandleBeginDrag(DraggableCommandUI command)
    {
        command.transform.SetParent(transform.root);
        command.GetCanvasGroup().blocksRaycasts = false;
        command.GetCanvasGroup().alpha = 0.6f;
    }

    public void HandleDrag(DraggableCommandUI command, PointerEventData eventData)
    {
        command.GetComponent<RectTransform>().position = eventData.position;
    }

    public void HandleEndDrag(DraggableCommandUI command, PointerEventData eventData)
    {
        command.GetCanvasGroup().blocksRaycasts = true;
        command.GetCanvasGroup().alpha = 1f;

        if (eventData.pointerEnter != null)
        {
            DropZone dropZone = eventData.pointerEnter.GetComponentInParent<DropZone>();
            if (dropZone != null)
            {
                if (command.transform.parent == transform.root)
                {
                    if (command.IsSpawned()) Destroy(command.gameObject);
                    else command.ReturnToOriginalParent();
                }
                return;
            }
        }

        if (command.IsSpawned()) Destroy(command.gameObject);
        else command.ReturnToOriginalParent();
    }

    public void SpawnCommandForDragging(GameObject commandPrefab, PointerEventData eventData)
    {
        GameObject newCommandInstance = Instantiate(commandPrefab, transform.root);
        DraggableCommandUI draggable = newCommandInstance.GetComponent<DraggableCommandUI>();
        draggable.MarkAsSpawned();

        eventData.pointerDrag = newCommandInstance;
        ExecuteEvents.Execute(newCommandInstance, eventData, ExecuteEvents.beginDragHandler);
    }
}