using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CommandExecutor : MonoBehaviour
{
    [Header("Referencias")]
    public Transform answerArea;
    public Button executeButton;

    [Header("Configuración")]
    public float delayBetweenCommands = 0.5f;

    private RobotController robot;
    private LevelData currentLevelData;
    private Puzzle puzzleController;

    public void SetPuzzleController(Puzzle controller)
    {
        this.puzzleController = controller;
    }

    public void ExecuteCommands()
    {
        // --- LÍNEA ACTUALIZADA ---
        robot = FindFirstObjectByType<RobotController>();
        if (robot == null)
        {
            Debug.LogError("¡No se encontró ningún RobotController en la escena!");
            return;
        }

        // --- LÍNEA ACTUALIZADA ---
        GridGenerator gridGenerator = FindFirstObjectByType<GridGenerator>();
        if (gridGenerator != null)
        {
            currentLevelData = gridGenerator.levelDataForWinCheck;
        }
        else
        {
            Debug.LogError("¡No se encontró ningún GridGenerator en la escena!");
            return;
        }

        List<CommandBlock> commandBlocks = new List<CommandBlock>();
        foreach (Transform child in answerArea)
        {
            if (child.GetComponent<CommandBlock>() != null)
            {
                commandBlocks.Add(child.GetComponent<CommandBlock>());
            }
        }

        StartCoroutine(ExecuteCommandsRoutine(commandBlocks));
    }

    private IEnumerator ExecuteCommandsRoutine(List<CommandBlock> commands)
    {
        if (executeButton != null) executeButton.interactable = false;

        Debug.Log("Iniciando ejecución de " + commands.Count + " comandos.");

        foreach (CommandBlock commandBlock in commands)
        {
            switch (commandBlock.commandType)
            {
                case CommandType.MoveForward:
                    robot.MoveForward();
                    break;
                case CommandType.TurnRight:
                    robot.TurnRight();
                    break;
                case CommandType.TurnLeft:
                    robot.TurnLeft();
                    break;
            }
            yield return new WaitForSeconds(delayBetweenCommands);
        }

        Debug.Log("Ejecución finalizada.");
        CheckWinCondition();

        if (executeButton != null) executeButton.interactable = true;
    }
    
    private void CheckWinCondition()
    {
        if (currentLevelData == null)
        {
            Debug.LogError("No se pudo comprobar la condición de victoria porque currentLevelData es nulo.");
            return;
        }

        // Obtenemos el tipo de tile en la posición final del robot
        TileType finalTile = currentLevelData.rows[robot.currentY].columns[robot.currentX];

        if (finalTile == TileType.Goal)
        {
            Debug.Log("¡¡¡HAS GANADO!!!");
            // Lógica de victoria
            if (puzzleController != null)
            {
                puzzleController.NotifyPuzzleSolved();
            }
        }
        else
        {
            Debug.Log("Casi... Sigue intentando.");
            // Lógica de "inténtalo de nuevo"
        }
    }
}