using UnityEngine;
using UnityEngine.Events;

public class Puzzle : MonoBehaviour, IInteractable
{
    [Header("Referencias de UI")]
    public GameObject puzzlePanel;
    public GameObject hotBar;

    [Header("Configuración del Puzle")]
    public LevelData levelToLoad;        // La "receta" del nivel que este objeto debe cargar.
    public GridGenerator gridGenerator;  // El "constructor" que dibujará la cuadrícula.
    public CommandExecutor commandExecutor;

    [Header("Evento de Victoria")]
    // Este evento se disparará cuando el puzle se resuelva con éxito.
    public UnityEvent onPuzzleSolved;


    private bool isPuzzlePanelOpen = false;

    void Awake()
    {
        // Nos aseguramos de que el commandExecutor sepa quién es su "jefe"
        if (commandExecutor != null)
        {
            commandExecutor.SetPuzzleController(this);
        }
    }

    void Start()
    {
        puzzlePanel.SetActive(false);
        hotBar.SetActive(true);
    }

    // Estas funciones de IInteractable ya están perfectas
    public bool CanInteract()
    {
        // El StartPuzzle() ya no es necesario aquí, lo haremos en TogglePuzzlePanel
        return true;
    }

    public void Interact()
    {
        TogglePuzzlePanel();
    }

    public void TogglePuzzlePanel()
    {
        if (!puzzlePanel.activeSelf && PauseController.IsGamePaused)
        {
            return;
        }

        isPuzzlePanelOpen = !isPuzzlePanelOpen;
        puzzlePanel.SetActive(isPuzzlePanelOpen);
        hotBar.SetActive(!isPuzzlePanelOpen);
        PauseController.SetPause(isPuzzlePanelOpen);

        // --- LÓGICA AÑADIDA ---
        // Si acabamos de ABRIR el panel...
        if (isPuzzlePanelOpen)
        {
            // ...y tenemos todo lo necesario asignado...
            if (levelToLoad != null && gridGenerator != null)
            {
                // ...le decimos al generador que construya la cuadrícula usando nuestros datos.
                gridGenerator.GenerateGrid(levelToLoad);
            }
            else
            {
                Debug.LogError("¡No se ha asignado un LevelData o un GridGenerator en el Inspector de " + gameObject.name + "!");
            }
        }
        else 
        {
            if (gridGenerator != null)
            {
                gridGenerator.ClearGrid();
            }
        }
        // -------------------------
    }

    public void NotifyPuzzleSolved()
    {
        Debug.Log("El puzle " + gameObject.name + " ha sido resuelto. ¡Disparando evento!");
        
        // Disparamos el evento. Cualquier cosa que esté escuchando, se activará.
        onPuzzleSolved.Invoke();

        // Opcional: Cerramos el panel del puzle automáticamente
        TogglePuzzlePanel();
    } 
}