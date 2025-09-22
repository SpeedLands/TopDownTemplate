using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    [SerializeField] Sprite selectedTabSprite;
    [SerializeField] Sprite unselectedTabSprite;
    public Image[] tabImages;
    public GameObject[] pages;

    void Start()
    {
        // Comprobación de seguridad al inicio
        if (tabImages.Length != pages.Length)
        {
            Debug.LogError("¡El número de imágenes de pestañas no coincide con el número de páginas! Revisa el Inspector.", this);
            return; // Detenemos la ejecución para evitar más errores
        }
        ActiveTab(0);
    }

    // Update está vacío, se puede quitar si no se va a usar.
    // void Update() { }

    public void ActiveTab(int tabNo)
    {
        // Comprobación para asegurarse de que el número de pestaña es válido
        if (tabNo < 0 || tabNo >= pages.Length)
        {
            Debug.LogError($"Se intentó activar una pestaña inválida: {tabNo}", this);
            return;
        }

        for (int i = 0; i < pages.Length; i++) // Es más seguro iterar sobre pages.Length
        {
            // --- LA SOLUCIÓN CLAVE: COMPROBAR ANTES DE USAR ---
            if (pages[i] != null)
            {
                pages[i].SetActive(false);
            }
            else
            {
                Debug.LogWarning($"La página en el índice {i} no está asignada en el Inspector.");
            }

            if (tabImages[i] != null)
            {
                tabImages[i].sprite = unselectedTabSprite;
            }
            else
            {
                Debug.LogWarning($"La imagen de la pestaña en el índice {i} no está asignada en el Inspector.");
            }
        }

        // Activamos la pestaña seleccionada, también con comprobaciones
        if (pages[tabNo] != null)
        {
            pages[tabNo].SetActive(true);
        }
        if (tabImages[tabNo] != null)
        {
            tabImages[tabNo].sprite = selectedTabSprite;
        }
    }
}