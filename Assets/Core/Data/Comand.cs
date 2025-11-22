using UnityEngine;

// Esto te permite crear nuevos comandos desde el menú de Assets en Unity.
[CreateAssetMenu(fileName = "Nuevo Comando", menuName = "Batalla/Comando")]
public class Comand : ScriptableObject
{
    [Header("Información Básica")]
    public string comandKeyword; // La palabra que el jugador debe escribir (ej: "atacar", "reparar")
    [TextArea] public string description; // La descripción que se muestra en la lista de comandos.
    public Sprite icon;

    [Header("Reglas del Comando")]
    public bool isBroken = false; // Regla #2: Si el comando está roto.
    public int maxUses = 1; // Regla #3: Cuántas veces se puede usar (puede ser expandido).
    [HideInInspector] public int currentUses; // Para llevar la cuenta de los usos actuales.

    // Puedes añadir más cosas aquí, como el poder del ataque, cuánto cura, etc.
    public int power;

    // Un método para resetear el estado del comando al inicio de una batalla.
    public void Reset()
    {
        currentUses = maxUses;
        isBroken = false;
    }
}