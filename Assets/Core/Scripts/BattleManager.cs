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
    [SerializeField] private PlayerHealth player;
    private PlayerStats playerStats; // Referencia a las estadísticas del jugador
    [SerializeField] private InventoryController inventory; // NUEVO: Referencia al inventario

    [Header("Configuración de Batalla")]
    [Tooltip("La lista de todos los comandos que el jugador ha desbloqueado.")]
    [SerializeField] private List<Comand> playerKnownCommands;
    [Tooltip("El número máximo de errores antes de perder la batalla.")]
    [SerializeField] private int maxErrors = 3;

    // --- Estado Interno de la Batalla ---
    private Enemy currentEnemy;
    private EnemyStats currentEnemyStats; // NUEVO: Referencia a las estadísticas del enemigo
    private PlayerBattleStats battlePlayerStats; // NUEVO: Estadísticas temporales para la batalla
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

        // --- NUEVO: Obtener referencia a PlayerStats ---
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("Player reference is not set in BattleManager!");
        }
    }

    // El punto de entrada. Este método es llamado por el enemigo o el jugador para iniciar el combate.
    public void StartBattle(Enemy enemy)
    {
        if (state != BattleState.Inactive) return; // Evita iniciar una batalla si ya hay una en curso

        this.currentEnemy = enemy;
        this.currentEnemyStats = enemy.GetComponent<EnemyStats>();

        if (currentEnemyStats == null)
        {
            Debug.LogError("Enemy is missing EnemyStats component! Aborting battle.");
            return;
        }

        // 1. Configurar la UI y el estado del mundo
        uiManager.ShowBattleUI(true);
        uiManager.UpdateEnemySprite(enemy.GetComponent<SpriteRenderer>().sprite);
        player.GetComponent<PlayerMovement>().enabled = false; // Desactivamos el movimiento del jugador

        // --- CAMBIO CLAVE PARA EVITAR FREEZING ---
        // En lugar de desactivar el GameObject, desactivamos sus componentes de física y renderizado.
        // Esto evita que el trigger/collision se dispare repetidamente si el jugador sigue en contacto.
        enemy.GetComponent<Collider2D>().enabled = false;
        enemy.GetComponent<Rigidbody2D>().simulated = false; // Detiene toda la simulación de física
        enemy.GetComponent<SpriteRenderer>().enabled = false; // Lo hace invisible en el mundo
        // enemy.gameObject.SetActive(false); // Ocultamos al enemigo del mundo (Forma antigua)

        // 2. Inicializar el estado de la batalla
        errorCount = 0;
        usedCommandsThisTurn = new List<Comand>();
        uiManager.UpdateErrorCount(errorCount);

        // --- NUEVO: Aplicar bonus de dados ---
        ApplyDiceBonuses();

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

        // --- REGLA DE REPARACIÓN ---
        // Si se usa "reparar", debe ser el único comando.
        if (rawInput.ToLower().Contains("reparar") && rawInput.Split(',').Length > 1 && battlePlayerStats.level < 5)
        {
            HandleCommandError("Error: El comando 'reparar' debe usarse solo (a menos que seas nivel 5+).");
            StartCoroutine(EndPlayerTurn());
            yield break;
        }

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
            // --- NUEVA LÓGICA PARA REPARAR ---
            if (input.StartsWith("reparar(") && input.EndsWith(")"))
            {
                // Extraemos el nombre del comando a reparar. Ej: de "reparar(atacar)" saca "atacar".
                string commandToRepairName = input.Substring(8, input.Length - 9).Trim();
                Comand commandToRepair = playerKnownCommands.Find(c => c.comandKeyword.ToLower() == commandToRepairName);

                if (commandToRepair != null)
                {
                    if (commandToRepair.isBroken)
                    {
                        commandToRepair.isBroken = false;
                        uiManager.LogToConsole($"¡Éxito! El comando '{commandToRepairName}' ha sido reparado.");
                    }
                    else
                    {
                        uiManager.LogToConsole($"El comando '{commandToRepairName}' no estaba roto.");
                    }
                }
                else
                {
                    HandleCommandError($"Error: No se encontró un comando llamado '{commandToRepairName}' para reparar.");
                }
                usedCommandsThisTurn.Add(playerKnownCommands.Find(c => c.comandKeyword == "reparar")); // Marcamos 'reparar' como usado.
            }
            else
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
                    // --- NUEVO: Probabilidad de fallo ---
                    if (Random.value < battlePlayerStats.GetCommandFailureChance())
                    {
                        commandToExecute.isBroken = true;
                        HandleCommandError($"¡Oh no! El comando '{input}' se ha roto por el uso.");
                    }
                    else
                    {
                        // ¡Éxito! Ejecutamos el comando
                        uiManager.LogToConsole($"Ejecutando: {commandToExecute.description}");
                        ExecuteCommandEffect(commandToExecute);
                    }
                    usedCommandsThisTurn.Add(commandToExecute);
                }
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
                // --- MODIFICADO: Usar el daño del jugador + el poder del comando ---
                int totalDamage = battlePlayerStats.baseDamage + command.power;
                Debug.Log($"Jugador ataca con poder total {totalDamage} ({battlePlayerStats.baseDamage} base + {command.power} del comando).");
                currentEnemyStats.TakeDamage(totalDamage);
                break;
            case "curar":
                // Lógica para curar al jugador
                player.Heal(command.power); // Asumiendo que Heal() toma cuartos de corazón
                break;
            case "reparar":
                // La lógica principal de reparar ya se maneja en ProcessCommandsRoutine.
                // Esta parte solo se ejecutaría si "reparar" fuera un comando simple sin argumentos,
                // lo cual ya no es el caso. Podemos dejarlo vacío o con un log.
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
        else if (currentEnemyStats.IsDead()) // Comprobar si el enemigo fue derrotado
        {
            state = BattleState.Won;
            EndBattle(true);
        }
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

        // --- IA del Enemigo Mejorada ---
        if (currentEnemyStats.isChargingSpecial)
        {
            // El enemigo estaba cargando, ¡ahora ataca!
            uiManager.LogToConsole($"¡{currentEnemy.name} desata su {currentEnemyStats.chargedAction.actionName}!");
            player.TakeDamage(currentEnemyStats.chargedAction.power);
            currentEnemyStats.isChargingSpecial = false;
        }
        else
        {
            // Elige una acción al azar basada en las probabilidades
            EnemyAction action = ChooseEnemyAction();
            if (action != null)
            {
                switch (action.type)
                {
                    case EnemyAction.ActionType.Attack:
                        uiManager.LogToConsole($"¡{currentEnemy.name} usa {action.actionName}!");
                        player.TakeDamage(action.power);
                        break;
                    case EnemyAction.ActionType.Heal:
                        uiManager.LogToConsole($"¡{currentEnemy.name} usa {action.actionName} y se cura!");
                        currentEnemyStats.Heal(action.power);
                        break;
                    case EnemyAction.ActionType.SpecialAttack:
                        // Aviso al jugador
                        uiManager.LogToConsole($"¡{currentEnemy.name} está cargando su {action.actionName}!");
                        currentEnemyStats.isChargingSpecial = true;
                        currentEnemyStats.chargedAction = action;
                        break;
                }
            }
        }
        // --- Fin de la IA ---

        yield return new WaitForSeconds(1.5f);

        // Comprobar si el jugador fue derrotado
        if (player.currentHealth <= 0)
        {
            state = BattleState.Lost;
            EndBattle(false);
        }
        else
        {
            state = BattleState.PlayerTurn;
            uiManager.LogToConsole("Tu turno. Escribe tus comandos.");
        }
    }

    /// <summary>
    /// Elige una acción para el enemigo basándose en las probabilidades definidas en EnemyStats.
    /// </summary>
    private EnemyAction ChooseEnemyAction()
    {
        if (currentEnemyStats.actions == null || currentEnemyStats.actions.Count == 0)
        {
            Debug.LogError("El enemigo no tiene acciones definidas.");
            return null;
        }

        float totalChance = currentEnemyStats.actions.Sum(a => a.chance);
        float randomPoint = Random.value * totalChance;

        foreach (var action in currentEnemyStats.actions)
        {
            if (randomPoint < action.chance)
            {
                return action;
            }
            randomPoint -= action.chance;
        }
        // Como fallback, si algo sale mal con las probabilidades, devuelve la primera acción
        return currentEnemyStats.actions[0];
    }

    /// <summary>
    /// Crea una copia temporal de las estadísticas del jugador y aplica los bonus de los dados del inventario.
    /// </summary>
    private void ApplyDiceBonuses()
    {
        // 1. Crear una copia de las estadísticas base usando el nuevo constructor
        battlePlayerStats = new PlayerBattleStats(playerStats);

        // 2. Recorrer el inventario y aplicar bonus
        if (inventory != null)
        {
            // --- CAMBIO: Usar GetInventoryItems() para obtener los datos ---
            // Esto es más seguro que iterar sobre la UI directamente.
            // Necesitamos una forma de obtener el 'Item' o 'DiceItem' desde los datos.
            // Por ahora, mantendré la iteración de la UI, pero esto debería mejorarse.
            foreach (Transform slotTransform in inventory.inventoryPanel.transform)
            {
                Slot slot = slotTransform.GetComponent<Slot>();
                if (slot != null && slot.currentItem != null)
                {
                    Item item = slot.currentItem.GetComponent<Item>();
                    if (item != null && item.diceData != null)
                    {
                        // Encontramos un dado, aplicamos sus bonus
                        battlePlayerStats.baseDamage += item.diceData.damageBonus;
                        battlePlayerStats.maxHealth += item.diceData.maxHealthBonus;
                        battlePlayerStats.failureChanceReduction += item.diceData.failureChanceReduction;
                        Debug.Log($"Aplicado bonus del dado: {item.diceData.itemName}");
                    }
                }
            }
        }

        // 3. (Opcional) Actualizar la UI de vida para reflejar la vida máxima con bonus
        player.GetComponent<PlayerHealth>().UpdateMaxHealthInBattle(battlePlayerStats.maxHealth);
    }

    private void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            uiManager.LogToConsole("¡VICTORIA!");
            // Dar recompensas, experiencia, etc.
            int xpGained = currentEnemyStats.experienceValue;
            playerStats.AddExperience(xpGained);
            uiManager.LogToConsole($"¡Has ganado {xpGained} puntos de experiencia!");
            Destroy(currentEnemy.gameObject);
        }
        else
        {
            uiManager.LogToConsole("Has sido derrotado...");
            // Lógica de Game Over, recargar checkpoint, etc.
            // Por ahora, solo reactivamos al enemigo para poder intentarlo de nuevo.
            currentEnemy.gameObject.SetActive(true);
        }

        // --- NUEVO: Restaurar la vida de la UI ---
        player.GetComponent<PlayerHealth>().RestoreHealthAfterBattle();

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