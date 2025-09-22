using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;

    void Start()
    {
        if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (interactableInRange != null)
            {
                interactableInRange.Interact();

                // Esta lógica es para ocultar el icono una vez que la interacción ha comenzado
                // (por ejemplo, un diálogo que no se puede volver a iniciar de inmediato).
                if (!interactableInRange.CanInteract())
                {
                    if (interactionIcon != null)
                    {
                        interactionIcon.SetActive(false);
                    }
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            if (interactionIcon != null)
            {
                interactionIcon.SetActive(true);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        // Evita que al salir de un trigger diferente se borre el interactuable actual.
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            if (interactionIcon != null)
            {
                interactionIcon.SetActive(false);
            }
        }
    }
}