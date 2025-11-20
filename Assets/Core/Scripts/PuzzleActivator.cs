using UnityEngine;

// This component is attached to objects in the game world (e.g., a terminal, a pedestal).
// It triggers the puzzle UI when the player interacts with it.
public class PuzzleActivator : MonoBehaviour, IInteractable // Assuming an IInteractable interface exists
{
    [Header("Puzzle Configuration")]
    [SerializeField] private LevelData puzzleToLoad;

    // --- IInteractable Implementation ---

    public bool CanInteract()
    {
        // Prevent interaction if the puzzle is already active or the game is paused.
        return PuzzleManager.Instance.CurrentState == PuzzleManager.PuzzleState.Inactive
               && !PauseController.IsGamePaused; // Assuming a PauseController exists
    }

    public void Interact()
    {
        if (CanInteract())
        {
            ActivatePuzzle();
        }
    }

    // --- Activation Logic ---

    private void ActivatePuzzle()
    {
        if (puzzleToLoad == null)
        {
            Debug.LogError("No LevelData assigned to this PuzzleActivator!", this);
            return;
        }

        // Load the puzzle data into the manager
        PuzzleManager.Instance.LoadPuzzle(puzzleToLoad);

        // Show the puzzle UI screen
        PuzzleUIController.Instance.ShowPuzzleScreen(true, puzzleToLoad);

        // You might also want to pause the game world or disable player movement here
        PauseController.SetPause(true); // Example
    }

    // This could be called from a "Close" button on the puzzle UI
    public void DeactivatePuzzle()
    {
         PuzzleManager.Instance.UnloadPuzzle();
         PuzzleUIController.Instance.ShowPuzzleScreen(false);
         PauseController.SetPause(false); // Example
    }
}

// NOTE: You will need a corresponding IInteractable interface for this to compile.
// public interface IInteractable
// {
//     bool CanInteract();
//     void Interact();
// }
