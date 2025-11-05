using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCommand : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentBeforeDrag;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (GetComponent<CanvasGroup>() == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        else
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentBeforeDrag = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        DropZone dropZone = eventData.pointerEnter?.GetComponent<DropZone>();

        if (dropZone != null)
        {
            if (dropZone.transform.childCount < dropZone.maxCommands)
            {
                transform.SetParent(dropZone.transform);
            }
            else
            {
                Debug.Log("¡Límite de comandos alcanzado! El comando será destruido.");
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}