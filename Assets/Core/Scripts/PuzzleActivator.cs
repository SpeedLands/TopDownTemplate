using UnityEngine;

// Este componente solo necesita una referencia al LevelData.
// Su única tarea es activar el puzzle a través del PuzzleManager.
public class PuzzleActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private LevelData puzzleToLoad;

    public bool CanInteract()
    {
        // Solo se puede interactuar si no hay otro puzzle activo.
        return PuzzleManager.Instance.CurrentState == PuzzleManager.PuzzleState.Inactive;
    }

    public void Interact()
    {
        if (CanInteract() && puzzleToLoad != null)
        {
            // Llama al singleton PuzzleManager para que se encargue de todo.
            PuzzleManager.Instance.LoadPuzzle(puzzleToLoad);
        }
        else
        {
            Debug.LogWarning("No se puede interactuar o no hay un LevelData asignado.", this);
        }
    }
}
