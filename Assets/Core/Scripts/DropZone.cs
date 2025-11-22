using UnityEngine;
using UnityEngine.EventSystems;

// This script remains on the "Execution Area" panel.
// Its sole responsibility is to validate and accept dropped commands.
public class DropZone : MonoBehaviour, IDropHandler
{
    public int maxCommands = 4;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        DraggableCommandUI draggable = droppedObject.GetComponent<DraggableCommandUI>();
        if (draggable == null) return;

        // Check if the drop zone has space
        if (transform.childCount < maxCommands)
        {
            // If there's space, accept the command by setting its parent.
            // The PuzzleUIController will see that the parent has changed and
            // will not destroy or move the object.
            draggable.transform.SetParent(this.transform);
        }
        else
        {
            Debug.Log("Command sequence is full. Cannot add more commands.");
            // If the drop zone is full, we do nothing. The PuzzleUIController
            // will see that the object's parent is still the root and handle it.
        }
    }
}