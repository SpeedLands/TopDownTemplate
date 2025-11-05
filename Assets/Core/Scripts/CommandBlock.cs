using UnityEngine;

// Enum para definir todos los tipos de comandos posibles
public enum CommandType
{
    MoveForward,
    TurnRight,
    TurnLeft
    // Aquí podrías añadir más en el futuro, como 'Jump', 'Activate', etc.
}

public class CommandBlock : MonoBehaviour
{
    public CommandType commandType;
}