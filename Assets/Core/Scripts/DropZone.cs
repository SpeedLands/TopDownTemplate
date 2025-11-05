using UnityEngine;
using UnityEngine.EventSystems;

// Este script ahora sirve como marcador Y como portero.
public class DropZone : MonoBehaviour
{
    // --- NUEVA LÍNEA ---
    // Hacemos esta variable pública para poder cambiar el límite desde el Inspector de Unity.
    public int maxCommands = 4;

    // Esta función se llama cuando un objeto DraggableCommand se suelta aquí.
    // La hemos añadido de nuevo desde la versión anterior.
    public void OnDrop(PointerEventData eventData)
    {
        DraggableCommand draggable = eventData.pointerDrag.GetComponent<DraggableCommand>();
        if (draggable == null) return; // Si lo que se soltó no es un comando, no hacemos nada.

        // --- LÓGICA DE LÍMITE ---
        // Comprobamos si el número de comandos ACTUALES es menor que nuestro límite.
        if (transform.childCount < maxCommands)
        {
            // Si hay espacio, aceptamos el comando.
            Debug.Log("Comando aceptado. Espacios ocupados: " + (transform.childCount + 1));
            draggable.transform.SetParent(this.transform);
        }
        else
        {
            // Si el área está llena, RECHAZAMOS el comando.
            // No hacemos nada aquí. El script DraggableCommand se encargará de destruirlo
            // porque no encontró un nuevo hogar.
            Debug.Log("¡Límite de comandos alcanzado! No se puede añadir más.");
            // Aquí podrías añadir un sonido de "error" para el jugador.
        }
    }
}