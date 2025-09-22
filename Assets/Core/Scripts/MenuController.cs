using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    private bool isMenuOpen = false;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Esta es la función que llamaremos desde el script del jugador
    public void ToggleMenu()
    {
        if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
        {
            // Si el juego está pausado, no abrimos el menú
            return;
        }
        isMenuOpen = !isMenuOpen;
        menuCanvas.SetActive(isMenuOpen);
        PauseController.SetPause(isMenuOpen);
    }

    // Devuelve si el menú está abierto o no, puede ser útil
    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }
}