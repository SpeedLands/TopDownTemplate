using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static PuzzleManager Instance { get; private set; }

    // --- State Machine ---
    public enum PuzzleState
    {
        Inactive,
        Playing,
        Executing,
        Won,
        Lost
    }
    public PuzzleState CurrentState { get; private set; }

    // --- Public Event ---
    // Any script can subscribe to this to be notified when a puzzle is solved.
    // The string parameter will be the ID of the completed puzzle.
    public static event Action<string> OnPuzzleCompleted;

    // --- Private State ---
    private LevelData currentLevelData;
    private int executionCount;

    void Awake()
    {
        // Initialize Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        CurrentState = PuzzleState.Inactive;
    }

    // --- Public Methods ---

    // Called by PuzzleActivator to start a new puzzle
    public void LoadPuzzle(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("Attempted to load a null LevelData!");
            return;
        }

        currentLevelData = levelData;
        executionCount = 0;
        CurrentState = PuzzleState.Playing;

        Debug.Log($"Puzzle '{currentLevelData.puzzleId}' loaded. You have {currentLevelData.maxExecutions} executions.");

        // Here you would also tell the GridGenerator to build the grid
        // and the PuzzleUIController to show the allowed commands.
    }

    // Called by PuzzleUIController when the "Execute" button is pressed
    public void ExecuteCommandSequence(List<Command> commands)
    {
        if (CurrentState != PuzzleState.Playing || currentLevelData == null)
        {
            Debug.LogWarning("Cannot execute commands right now. State: " + CurrentState);
            return;
        }

        if (executionCount >= currentLevelData.maxExecutions)
        {
            Debug.Log("Execution limit reached. Puzzle failed.");
            CurrentState = PuzzleState.Lost;
            // Here you would update the UI to show a "You Lost" message
            return;
        }

        executionCount++;
        Debug.Log($"Execution #{executionCount}/{currentLevelData.maxExecutions}");

        // Start the execution coroutine
        StartCoroutine(ProcessSequence(commands));
    }

    // Coroutine to execute commands one by one
    private IEnumerator ProcessSequence(List<Command> commands)
    {
        CurrentState = PuzzleState.Executing;
        // Here you would disable the "Execute" button in the UI

        foreach (var command in commands)
        {
            // --- Placeholder for command execution ---
            // command.Execute();
            Debug.Log("Executing command: " + command.GetType().Name);

            // Wait for a short time to give visual feedback
            yield return new WaitForSeconds(0.5f);
        }

        // --- Placeholder for win condition check ---
        bool puzzleIsSolved = CheckWinCondition();

        if (puzzleIsSolved)
        {
            CurrentState = PuzzleState.Won;
            Debug.Log($"Puzzle '{currentLevelData.puzzleId}' SOLVED!");

            // Invoke the global event
            OnPuzzleCompleted?.Invoke(currentLevelData.puzzleId);

            // Here you would show a "You Won" message and maybe close the puzzle screen
        }
        else
        {
            // If not solved, return to the playing state to allow for another attempt
            CurrentState = PuzzleState.Playing;
            Debug.Log("Sequence finished. Win condition not met.");
             // Here you might want to reset the player/robot to the start position
        }
    }

    private bool CheckWinCondition()
    {
        // --- Placeholder ---
        // In the real implementation, you would check if the player/robot
        // is on the 'Goal' tile. For now, we'll just simulate a win on the first try.
        return executionCount == 1;
    }

    // Called when the puzzle screen is closed
    public void UnloadPuzzle()
    {
        CurrentState = PuzzleState.Inactive;
        currentLevelData = null;
        Debug.Log("Puzzle unloaded.");
        // Here you would clear the grid and command sequence UI
    }
}
