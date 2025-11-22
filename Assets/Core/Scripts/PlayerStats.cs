using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Attributes")]
    [Tooltip("The player's current level.")]
    public int level = 1;

    [Tooltip("The player's programming skill level, affecting command failure rates.")]
    public int programmingSkill = 1;

    [Tooltip("The player's maximum health in quarter hearts.")]
    public int maxHealth = 12; // Ejemplo: 3 corazones = 12 cuartos

    [Tooltip("The base damage the player deals with attacks.")]
    public int baseDamage = 1;

    [Header("Progression")]
    [Tooltip("The current experience points of the player.")]
    public int currentExperience = 0;

    [Tooltip("The experience points needed to reach the next level.")]
    public int experienceForNextLevel = 100;

    // --- Eventos ---
    // Se dispara cuando las estadísticas del jugador cambian (ej. al subir de nivel)
    public event System.Action OnStatsChanged;

    /// <summary>
    /// Adds experience points to the player and handles leveling up.
    /// </summary>
    /// <param name="amount">The amount of experience to add.</param>
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log($"Player gained {amount} XP. Total XP: {currentExperience}/{experienceForNextLevel}");

        // Check if the player has enough experience to level up
        while (currentExperience >= experienceForNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExperience -= experienceForNextLevel;
        level++;

        // --- Improve stats on level up ---
        programmingSkill++;
        maxHealth += 4; // Aumenta un corazón (4 cuartos)
        baseDamage++;
        experienceForNextLevel = Mathf.RoundToInt(experienceForNextLevel * 1.5f); // Increase XP requirement for next level

        Debug.Log($"LEVEL UP! Player is now level {level}.");

        // Notify other systems that stats have changed
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Gets the current probability of a command failing.
    /// The higher the programming skill, the lower the chance of failure.
    /// </summary>
    /// <returns>A float value between 0.0 and 1.0 representing the failure chance.</returns>
    public float GetCommandFailureChance()
    {
        // Example formula: Starts at 25% and decreases with skill.
        // Never goes below 5%.
        return Mathf.Max(0.05f, 0.25f - (programmingSkill * 0.02f));
    }
}