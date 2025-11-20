using UnityEngine;
using UnityEngine.EventSystems;

// This component is attached to the command prefab UI elements.
// It acts as a lightweight communicator with the central PuzzleUIController.
[RequireComponent(typeof(CanvasGroup))]
public class DraggableCommandUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentBeforeDrag;
    private CanvasGroup canvasGroup;
    private bool wasSpawned = false; // Flag to check if this was instantiated by a spawner

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentBeforeDrag = transform.parent;
        PuzzleUIController.Instance.HandleBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        PuzzleUIController.Instance.HandleDrag(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PuzzleUIController.Instance.HandleEndDrag(this, eventData);
    }

    public CanvasGroup GetCanvasGroup()
    {
        return canvasGroup;
    }

    // --- New helper methods for PuzzleUIController ---
    public void MarkAsSpawned()
    {
        wasSpawned = true;
    }

    public bool IsSpawned()
    {
        return wasSpawned;
    }

    public void ReturnToOriginalParent()
    {
        transform.SetParent(parentBeforeDrag);
    }
}
