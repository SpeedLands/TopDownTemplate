using UnityEngine;

// An example of a component that listens for a puzzle completion event.
// This script would be placed on a door object in the game world.
public class DoorController : MonoBehaviour
{
    [Header("Event Listening")]
    [Tooltip("The unique ID of the puzzle that should trigger this door to open.")]
    [SerializeField] private string puzzleIdToListenFor;

    // --- Unity Lifecycle Methods ---

    private void OnEnable()
    {
        // Subscribe to the static event when this component becomes active.
        PuzzleManager.OnPuzzleCompleted += HandlePuzzleCompleted;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when the component is disabled or destroyed
        // to prevent memory leaks.
        PuzzleManager.OnPuzzleCompleted -= HandlePuzzleCompleted;
    }

    // --- Event Handler ---

    private void HandlePuzzleCompleted(string completedPuzzleId)
    {
        // Check if the ID of the completed puzzle matches the one we're listening for.
        if (completedPuzzleId == puzzleIdToListenFor)
        {
            Debug.Log($"Door '{gameObject.name}' received the correct puzzle ID ('{completedPuzzleId}'). Opening the door!");
            OpenDoor();
        }
    }

    // --- Door Logic ---

    private void OpenDoor()
    {
        // Here you would implement the actual logic to open the door.
        // For example, you might play an animation, disable a collider,
        // or move the door's transform.

        // For this example, we'll just deactivate the GameObject.
        gameObject.SetActive(false);
    }
}