using UnityEngine;
using UnityEngine.EventSystems;

public class CommandSpawner : MonoBehaviour, IPointerDownHandler
{
    // Arrastra aquí el PREFAB REAL del comando que quieres generar
    // (el que tiene DraggableCommand.cs y CommandBlock.cs)
    public GameObject commandPrefab;

    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. Crear una nueva instancia del prefab del comando
        GameObject newCommandInstance = Instantiate(commandPrefab, transform.root);

        // 2. Forzar el inicio del arrastre en la nueva instancia
        // Le pasamos el control del evento del ratón al nuevo objeto
        eventData.pointerDrag = newCommandInstance;
    }
}