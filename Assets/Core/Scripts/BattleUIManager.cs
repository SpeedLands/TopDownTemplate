using UnityEngine;
using UnityEngine.UI;
using TMPro; // ¡MUY IMPORTANTE! Añade esto para usar TextMeshPro.

public class BattleUIManager : MonoBehaviour
{
    [Header("Referencias Principales")]
    [SerializeField] private GameObject battleUIPanel; // El objeto raíz "RPGBattle"
    [SerializeField] private Image enemyImage; // El objeto "Image" que mostrará al enemigo
    [SerializeField] private TMP_InputField commandInput; // Tu objeto "ComandInput"
    [SerializeField] private Button commandListButton; // Tu "ComandButton"
    [SerializeField] private Button executeButton; // Tu "ExecuteButton"

    [Header("Contenedor de Información (InfoContainer)")]
    [SerializeField] private TMP_Text playerNameText; // El primer Text (TMP) en InfoContainer
    [SerializeField] private TMP_Text playerLevelText; // El segundo Text (TMP) en InfoContainer
    [SerializeField] private HeartUI heartUI; // La referencia a tu script de corazones que va en HealthContainer

    [Header("Contenedor de Estado (EstateContainer)")]
    [SerializeField] private Transform statusEffectContainer; // Tu objeto "EstateInfo", para añadir iconos de estado

    [Header("Contenedor de Errores (ErrorContainer)")]
    [SerializeField] private TMP_Text errorCountText; // El objeto "ErrorInfo" que mostrará el número de errores

    [Header("Consola de Depuración")]
    [SerializeField] private TMP_Text consoleText; // El texto de la consola (ver nota abajo)

    void Start()
    {
        // Asignamos las funciones a los botones
        executeButton.onClick.AddListener(OnExecutePressed);
        // commandListButton.onClick.AddListener(OnCommandListPressed); // Lo prepararemos para el futuro

        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }
    }

    private void OnExecutePressed() 
    {
        string playerInput = commandInput.text;
        if (!string.IsNullOrEmpty(playerInput))
        {
            BattleManager.Instance.ProcessPlayerCommands(playerInput);
            commandInput.text = ""; // Limpiamos el campo de texto
            commandInput.ActivateInputField(); // Reactivamos el campo para que el jugador pueda seguir escribiendo
        }
    }

    // --- MÉTODOS PÚBLICOS PARA QUE EL BATTLEMANAGER LOS USE ---

    public void ShowBattleUI(bool show)
    {
        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(show);
        }
    }

    public void UpdateEnemySprite(Sprite sprite)
    {
        if (enemyImage != null)
        {
            enemyImage.sprite = sprite;
        }
    }

    public void LogToConsole(string message)
    {
        if (consoleText != null)
        {
            consoleText.text = message;
        }
    }

    public void UpdateErrorCount(int count)
    {
        if (errorCountText != null)
        {
            errorCountText.text = count.ToString(); // Actualizamos el contador de errores
        }
    }

    public void UpdatePlayerInfo(string playerName, int level)
    {
        if (playerNameText != null) playerNameText.text = playerName;
        if (playerLevelText != null) playerLevelText.text = "LV. " + level;
    }
}