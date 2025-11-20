using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [Header("System References")]
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private PuzzleUIController puzzleUIController; // Referencia al controlador de UI

    public enum PuzzleState { Inactive, Playing, Executing, Won, Lost }
    public PuzzleState CurrentState { get; private set; }

    public static event Action<string> OnPuzzleCompleted;

    private LevelData currentLevelData;
    private int executionCount;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        CurrentState = PuzzleState.Inactive;
    }

    public void LoadPuzzle(LevelData levelData)
    {
        if (levelData == null) return;

        currentLevelData = levelData;
        executionCount = 0;
        CurrentState = PuzzleState.Playing;

        if (gridGenerator != null) gridGenerator.GenerateGrid(currentLevelData);

        // --- LÓGICA DE UI CENTRALIZADA ---
        if (puzzleUIController != null) puzzleUIController.ShowPuzzleScreen(true, currentLevelData);
        else Debug.LogError("PuzzleUIController no está asignado en PuzzleManager!");
        // ----------------------------------

        Debug.Log($"Puzzle '{currentLevelData.puzzleId}' cargado. Tienes {currentLevelData.maxExecutions} ejecuciones.");
        PauseController.SetPause(true); // Pausa el juego
    }

    public void UnloadPuzzle()
    {
        if (gridGenerator != null) gridGenerator.ClearGrid();

        // --- LÓGICA DE UI CENTRALIZADA ---
        if (puzzleUIController != null) puzzleUIController.ShowPuzzleScreen(false);
        // ----------------------------------

        CurrentState = PuzzleState.Inactive;
        currentLevelData = null;
        Debug.Log("Puzzle descargado.");
        PauseController.SetPause(false); // Reanuda el juego
    }

    public void ExecuteCommandSequence(List<Command> commands)
    {
        if (CurrentState != PuzzleState.Playing || currentLevelData == null) return;
        if (executionCount >= currentLevelData.maxExecutions)
        {
            CurrentState = PuzzleState.Lost;
            return;
        }
        executionCount++;
        StartCoroutine(ProcessSequence(commands));
    }

    private IEnumerator ProcessSequence(List<Command> commands)
    {
        CurrentState = PuzzleState.Executing;
        RobotController robot = gridGenerator.Robot; // Obtenemos la referencia al robot

        if (robot == null)
        {
            Debug.LogError("No se encontró el RobotController para ejecutar los comandos.");
            CurrentState = PuzzleState.Playing; // Volvemos al estado anterior
            yield break; // Salimos de la corutina
        }

        foreach (var command in commands)
        {
            command.Execute(robot); // Ejecutamos el comando en el robot
            yield return new WaitForSeconds(0.5f); // Espera para visualización
        }

        bool puzzleIsSolved = CheckWinCondition();
        if (puzzleIsSolved)
        {
            CurrentState = PuzzleState.Won;
            OnPuzzleCompleted?.Invoke(currentLevelData.puzzleId);
            // Cierra automáticamente el puzzle al ganar
            UnloadPuzzle();
        }
        else
        {
            CurrentState = PuzzleState.Playing;
        }
    }

    private bool CheckWinCondition()
    {
        RobotController robot = gridGenerator.Robot;
        if (robot == null) return false;

        // La condición de victoria es que el robot esté en la casilla 'Goal'
        int robotX = robot.currentX;
        int robotY = robot.currentY;

        return currentLevelData.rows[robotY].columns[robotX] == TileType.Goal;
    }
}
