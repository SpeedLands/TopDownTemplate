using UnityEngine;
using UnityEngine.EventSystems;

// [RequireComponent(typeof(CanvasGroup))] // Descomenta esta línea si quieres que Unity añada el CanvasGroup automáticamente.
public class ComandDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake() // Usamos Awake para asegurarnos de que las referencias estén listas antes de Start.
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
        {
            // Si el objeto no tiene CanvasGroup, lo añadimos y advertimos.
            Debug.LogWarning("El objeto " + name + " no tenía un CanvasGroup. Se ha añadido uno automáticamente.", gameObject);
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // Lo movemos a la raíz del Canvas para que se renderice por encima de todo.
        
        canvasGroup.blocksRaycasts = false; // Permite que el raycast pase a través del objeto para detectar lo que hay debajo.
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Sigue la posición del ratón/dedo.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1. Restaurar la apariencia del objeto.
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // 2. Encontrar sobre qué Slot se ha soltado el objeto.
        Slot dropSlot = null;
        if (eventData.pointerEnter != null)
        {
            dropSlot = eventData.pointerEnter.GetComponent<Slot>();
            if (dropSlot == null)
            {
                // Si no lo encontramos directamente, buscamos en el padre (útil si el objeto tiene hijos).
                dropSlot = eventData.pointerEnter.GetComponentInParent<Slot>();
            }
        }
        
        // 3. Encontrar el Slot de origen (si existía).
        Slot originalSlot = originalParent.GetComponent<Slot>();

        // 4. Decidir qué hacer basándonos en si el origen y el destino son válidos.

        if (dropSlot == null)
        {
            // --- CASO A: Se soltó en un lugar inválido ---
            Debug.Log("Soltado en un área no válida.");
            HandleInvalidDrop(originalSlot);
            return;
        }

        if (dropSlot.currentItem == null)
        {
            // --- CASO B: Se soltó en un SLOT VACÍO ---
            Debug.Log("Moviendo " + gameObject.name + " al slot vacío: " + dropSlot.name);
            MoveToEmptySlot(originalSlot, dropSlot);
        }
        else
        {
            // --- CASO C: Se soltó en un SLOT OCUPADO ---
            Debug.Log("Intentando intercambiar con el item del slot: " + dropSlot.name);
            HandleSwap(originalSlot, dropSlot);
        }
    }
    
    private void MoveToEmptySlot(Slot origin, Slot destination)
    {
        // Liberar el slot de origen, si veníamos de uno.
        if (origin != null)
        {
            origin.currentItem = null;
        }

        // Mover nuestro objeto al slot de destino.
        transform.SetParent(destination.transform);
        destination.currentItem = gameObject;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void HandleSwap(Slot origin, Slot destination)
    {
        if (origin == null)
        {
            // No se puede intercambiar si venimos de la "paleta" (un lugar sin Slot).
            // En este caso, simplemente volvemos al origen.
            Debug.Log("No se puede intercambiar desde la paleta. Cancelando.");
            HandleInvalidDrop(origin);
            return;
        }
        
        // Guardamos la referencia al otro objeto.
        GameObject otherItem = destination.currentItem;

        // Mover el otro objeto a nuestro slot original.
        otherItem.transform.SetParent(origin.transform);
        otherItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        origin.currentItem = otherItem;

        // Mover nuestro objeto al slot de destino.
        transform.SetParent(destination.transform);
        destination.currentItem = gameObject;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void HandleInvalidDrop(Slot origin)
    {
        if (origin != null)
        {
            // Si veníamos de un slot, volvemos a él.
            transform.SetParent(origin.transform);
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            // Si veníamos de la paleta y no caemos en un slot válido,
            // lo más lógico es destruir el objeto "temporal".
            // Si tus comandos de la paleta no son temporales, cambia esta lógica.
            Debug.Log("Objeto de la paleta soltado en un lugar inválido. Destruyendo.");
            Destroy(gameObject);
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Lógica para el clic derecho, por ejemplo, eliminar el comando del slot.
            Debug.Log("Clic derecho en " + gameObject.name);
        }
    }
}