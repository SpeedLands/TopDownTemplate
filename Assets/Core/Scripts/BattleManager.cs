using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Necesario para usar funciones como .Find() y .Trim()

public class BattleManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    // Esto nos permite acceder al BattleManager desde cualquier script
    // de forma sencilla usando BattleManager.Instance
    public static BattleManager Instance;

    [Header("Referencias Principales")]
    [Tooltip("Arrastra aquí el GameObject que tiene el script BattleUIManager.")]
    [SerializeField] private BattleUIManager uiManager;
    [Tooltip("Arrastra aquí el GameObject del Jugador.")]
    [SerializeField] private PlayerHealth player; // Usaremos esto para curar o dañar al jugador

    [Header("Configuración de Batalla")]
    [Tooltip("La lista de todos los comandos que el jugador ha desbloqueado.")]
    [SerializeField] private List<Comand> playerKnownCommands;
    [Tooltip("El número máximo de errores antes de perder la batalla.")]
    [SerializeField] private int maxErrors = 3;

    // --- Estado Interno de la Batalla ---
    private Enemy currentEnemy;
    private List<Comand> usedCommandsThisTurn;
    private int errorCount;

    // El State Machine que controla el flujo de la batalla
    public enum BattleState { Inactive, PlayerTurn, Processing, EnemyTurn, Won, Lost }
    private BattleState state;

    void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // El punto de entrada. Este método es llamado por el enemigo o el jugador para iniciar el combate.
    public void StartBattle(Enemy enemy)
    {
        if (state != BattleState.Inactive) return; // Evita iniciar una batalla si ya hay una en curso

        this.currentEnemy = enemy;

        // 1. Configurar la UI y el estado del mundo
        uiManager.ShowBattleUI(true);
        uiManager.UpdateEnemySprite(enemy.GetComponent<SpriteRenderer>().sprite);
        player.GetComponent<PlayerMovement>().enabled = false; // Desactivamos el movimiento del jugador
        enemy.gameObject.SetActive(false); // Ocultamos al enemigo del mundo

        // 2. Inicializar el estado de la batalla
        errorCount = 0;
        usedCommandsThisTurn = new List<Comand>();
        uiManager.UpdateErrorCount(errorCount);
        // Reseteamos el estado de los comandos (por si estaban rotos de una batalla anterior)
        foreach (var comand in playerKnownCommands)
        {
            comand.Reset();
        }

        // 3. Iniciar el primer turno
        state = BattleState.PlayerTurn;
        // uiManager.LogToConsole("¡Un " + enemy.name + " te desafía! Escribe tus comandos.");
        
        // Actualizamos la info del jugador en la UI
        // (Aquí deberías obtener el nombre y nivel real de un script de stats del jugador)
        uiManager.UpdatePlayerInfo("Héroe", 1);
    }

    // Este método es llamado por el BattleUIManager cuando el jugador presiona "Ejecutar".
    public void ProcessPlayerCommands(string rawInput)
    {
        if (state != BattleState.PlayerTurn) return; // Solo procesamos si es el turno del jugador

        state = BattleState.Processing;
        usedCommandsThisTurn.Clear();
        uiManager.LogToConsole("Procesando...");

        // Iniciamos una corrutina para procesar los comandos con pausas
        StartCoroutine(ProcessCommandsRoutine(rawInput));
    }

    private IEnumerator ProcessCommandsRoutine(string rawInput)
    {
        yield return new WaitForSeconds(0.5f); // Pequeña pausa dramática

        // 1. Separamos los comandos escritos por el jugador (asumiendo que se separan por comas)
        string[] commandInputs = rawInput.Split(',').Select(cmd => cmd.Trim().ToLower()).ToArray();

        // 2. Validamos la cantidad de comandos (Regla #7)
        if (commandInputs.Length > 3)
        {
            HandleCommandError("Error: Máximo 3 comandos por turno.");
            StartCoroutine(EndPlayerTurn());
            yield break; // Termina la corrutina aquí
        }

        // 3. Procesamos cada comando
        foreach (string input in commandInputs)
        {
            Comand commandToExecute = playerKnownCommands.Find(c => c.comandKeyword.ToLower() == input);

            // --- Aplicamos las Reglas ---
            if (commandToExecute == null) // Regla #6: Comando no existe
            {
                HandleCommandError($"Error: Comando '{input}' no reconocido.");
            }
            else if (usedCommandsThisTurn.Contains(commandToExecute)) // Regla #1: Comando ya usado
            {
                HandleCommandError($"Error: Comando '{input}' ya fue usado este turno.");
            }
            else if (commandToExecute.isBroken) // Regla #2: Comando roto
            {
                HandleCommandError($"Error: Comando '{input}' está roto. Necesita reparación.");
            }
            else
            {
                // ¡Éxito! Ejecutamos el comando
                uiManager.LogToConsole($"Ejecutando: {commandToExecute.description}");
                ExecuteCommandEffect(commandToExecute);
                usedCommandsThisTurn.Add(commandToExecute);
            }
            yield return new WaitForSeconds(1f); // Pausa entre cada comando ejecutado
        }

        // 4. Pasamos al siguiente turno
        StartCoroutine(EndPlayerTurn());
    }

    private void ExecuteCommandEffect(Comand command)
    {
        // Aquí es donde la acción del comando tiene lugar
        switch (command.comandKeyword.ToLower())
        {
            case "atacar":
                // Lógica para dañar al enemigo
                Debug.Log("Jugador ataca con poder " + command.power);
                // currentEnemy.TakeDamage(command.power); // Necesitarías un script de vida en el enemigo
                break;
            case "curar":
                // Lógica para curar al jugador
                // player.Heal(command.power); // Asumiendo que Heal() toma cuartos de corazón
                break;
            case "reparar":
                // Lógica para reparar (más compleja, para el futuro)
                uiManager.LogToConsole("Función de reparar no implementada aún.");
                break;
        }
    }

    private void HandleCommandError(string message)
    {
        errorCount++;
        uiManager.UpdateErrorCount(errorCount);
        uiManager.LogToConsole(message);

        // Regla #10: Si se acumulan errores, pierdes
        if (errorCount >= maxErrors)
        {
            state = BattleState.Lost;
        }
    }

    private IEnumerator EndPlayerTurn()
    {
        uiManager.LogToConsole("Fin del turno del jugador.");
        yield return new WaitForSeconds(1.5f);

        if (state == BattleState.Lost)
        {
            EndBattle(false); // El jugador perdió por errores
        }
        // else if (currentEnemy.IsDead()) // Comprobar si el enemigo fue derrotado
        // {
        //     state = BattleState.Won;
        //     EndBattle(true);
        // }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyTurn;
        uiManager.LogToConsole("Turno del enemigo...");
        yield return new WaitForSeconds(1.5f);

        // --- IA del Enemigo (muy simple por ahora) ---
        uiManager.LogToConsole("¡El " + currentEnemy.name + " ataca!");
        player.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver); // Daña 1 cuarto de corazón
        // --- Fin de la IA ---

        yield return new WaitForSeconds(1.5f);

        // if (player.IsDead()) // Comprobar si el jugador fue derrotado
        // {
        //     state = BattleState.Lost;
        //     EndBattle(false);
        // }
        // else
        // {
            state = BattleState.PlayerTurn;
            uiManager.LogToConsole("Tu turno. Escribe tus comandos.");
        // }
    }

    private void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            uiManager.LogToConsole("¡VICTORIA!");
            // Dar recompensas, experiencia, etc.
            Destroy(currentEnemy.gameObject);
        }
        else
        {
            uiManager.LogToConsole("Has sido derrotado...");
            // Lógica de Game Over, recargar checkpoint, etc.
            // Por ahora, solo reactivamos al enemigo para poder intentarlo de nuevo.
            currentEnemy.gameObject.SetActive(true);
        }

        // Esperar unos segundos y luego volver al mundo
        StartCoroutine(ReturnToWorld());
    }

    private IEnumerator ReturnToWorld()
    {
        yield return new WaitForSeconds(3f); // Espera para que el jugador lea el resultado
        uiManager.ShowBattleUI(false);
        player.GetComponent<PlayerMovement>().enabled = true;
        state = BattleState.Inactive;
    }
}