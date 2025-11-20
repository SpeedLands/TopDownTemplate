using UnityEngine;

// This component is attached to the Command UI prefabs.
// It holds the data necessary to link the UI element to a specific command logic class.
public class CommandUIMetadata : MonoBehaviour
{
    // We will use an enum to define the command type. This keeps it simple and avoids
    // complex reflection or string-based lookups. The PuzzleUIController will use
    // this enum to instantiate the correct Command logic object.
    public CommandType commandType;
}

// We redefine the CommandType enum here. It will be expanded upon in the next step.
public enum CommandType
{
    MoveForward,
    TurnRight,
    TurnLeft,
    ToggleSpikes // Added the new command type
}
