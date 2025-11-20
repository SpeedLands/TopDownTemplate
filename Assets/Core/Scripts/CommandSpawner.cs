using UnityEngine;
using UnityEngine.EventSystems;

// This component sits on the command buttons in the palette.
// When clicked, it tells the PuzzleUIController to spawn a new command for dragging.
public class CommandSpawner : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("The command UI prefab to be spawned.")]
    public GameObject commandPrefab;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (commandPrefab == null)
        {
            Debug.LogError("Command Prefab is not set on this spawner!", this);
            return;
        }

        // Delegate the spawning and drag initiation to the central controller.
        PuzzleUIController.Instance.SpawnCommandForDragging(commandPrefab, eventData);
    }
}
