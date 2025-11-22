using UnityEngine;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Attributes")]
    [Tooltip("The enemy's maximum health.")]
    public int maxHealth = 20;

    [Tooltip("How much experience this enemy awards upon defeat.")]
    public int experienceValue = 50;

    [Tooltip("The current health of the enemy.")]
    public int currentHealth;

    [Header("Combat Actions")]
    [Tooltip("A list of actions the enemy can perform during its turn.")]
    public List<EnemyAction> actions;

    // --- State for Multi-Turn Actions ---
    [HideInInspector] public bool isChargingSpecial = false;
    [HideInInspector] public EnemyAction chargedAction;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Deals damage to the enemy and checks for death.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to inflict.</param>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage, current health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // The BattleManager will handle the death logic.
        }
    }

    /// <summary>
    /// Heals the enemy for a specific amount.
    /// </summary>
    /// <param name="healAmount">The amount of health to restore.</param>
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"Enemy healed for {healAmount}, current health: {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// Checks if the enemy has been defeated.
    /// </summary>
    /// <returns>True if the enemy's health is 0 or less.</returns>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}

// --- Defines a single action an enemy can take ---
[System.Serializable]
public class EnemyAction
{
    public enum ActionType { Attack, Heal, SpecialAttack }

    public string actionName;
    public ActionType type;
    public int power; // Can be damage for attacks, heal amount for heals, etc.
    [Range(0f, 1f)]
    public float chance = 1f; // The probability of this action being chosen.
}
