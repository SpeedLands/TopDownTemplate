# Puzzle System Testing Guide

This guide outlines the steps to set up a test scene in the Unity Editor to verify the complete workflow of the new puzzle system.

## Scene Setup Checklist

1.  **Core Systems:**
    *   Ensure there is a standard `EventSystem` in your scene (`GameObject -> UI -> Event System`).
    *   Create an empty GameObject named `_SYSTEMS`.
    *   Attach the `PuzzleManager.cs` script to `_SYSTEMS`.
    *   Attach the `PuzzleUIController.cs` script to your main Canvas or a child of `_SYSTEMS`.

2.  **UI Setup (`PuzzleScreen`):**
    *   Make sure your main `Canvas` has a `Graphic Raycaster` component.
    *   Assign all the necessary UI elements to the `PuzzleUIController` in the Inspector:
        *   `Puzzle Screen`: The parent panel for the entire puzzle UI.
        *   `Command Palette`: The `Transform` where allowed command buttons will be instantiated.
        *   `Execution Area`: The `Transform` with the `DropZone.cs` component where commands are placed.
        *   `Execute Button`: The `Button` that triggers the `OnExecuteButtonPressed` function.

3.  **Create a `LevelData` Asset:**
    *   In your Project window, right-click and go to `Create -> Puzzle -> Level Data`.
    *   Name this asset `TestLevel_Door`.
    *   Configure the `TestLevel_Door` asset:
        *   **Puzzle ID:** `DoorOpener`
        *   **Max Executions:** `3`
        *   **Allowed Commands:** Create a few command UI prefabs (e.g., `MoveForward_Prefab`, `TurnRight_Prefab`). Attach `DraggableCommandUI.cs` and `CommandUIMetadata.cs` to them. Assign the correct `CommandType` in the metadata. Drag these prefabs into the `Allowed Commands` list.
        *   **Grid Configuration:** Set up a simple grid layout.

4.  **Game World Objects:**
    *   **Puzzle Activator:**
        *   Create a simple 3D object (like a Cube) in your scene and name it `PuzzleTerminal`.
        *   Attach the `PuzzleActivator.cs` script to it.
        *   Ensure it has a `Collider` set to be a `Trigger`.
        *   In the Inspector for `PuzzleActivator`, drag your `TestLevel_Door` asset into the `Puzzle To Load` field.
    *   **Door:**
        *   Create another 3D object (like a Cube) and name it `SecretDoor`.
        *   Attach the `DoorController.cs` script to it.
        *   In the Inspector for `DoorController`, set the `Puzzle Id To Listen For` to `DoorOpener`.

## Verification Workflow

1.  **Enter Play Mode.**
2.  **Approach the `PuzzleTerminal`**. Your player character should be able to trigger the `IInteractable` interface.
3.  **Interact with the `PuzzleTerminal`**. The `PuzzleScreen` should appear, and the game world should pause.
4.  **Check the Command Palette**. It should only contain the commands you specified in the `TestLevel_Door` asset.
5.  **Drag and Drop Test:**
    *   Drag commands from the palette to the execution area.
    *   Reorder the commands within the execution area.
    *   Drag a command out of the execution area to delete it.
6.  **Execute the Sequence:**
    *   Create a simple sequence (e.g., one `MoveForward` command).
    *   Press the `Execute` button.
7.  **Verify Win Condition:**
    *   Because the `PuzzleManager`'s `CheckWinCondition()` is currently a placeholder that returns `true` on the first execution, the puzzle should immediately be solved.
    *   You should see the debug log: `"Puzzle 'DoorOpener' SOLVED!"`.
8.  **Verify Event Listening:**
    *   You should see the debug log: `"Door 'SecretDoor' received the correct puzzle ID ('DoorOpener'). Opening the door!"`.
    *   The `SecretDoor` GameObject should be deactivated in the scene.
9.  **Close the Puzzle Screen** (you may need to add a temporary "Close" button that calls `PuzzleActivator.DeactivatePuzzle()`). The UI should disappear and the game should unpause.

Following these steps will confirm that the core logic, UI, command system, and game world integration are all functioning as intended.
