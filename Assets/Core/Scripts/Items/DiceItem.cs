using UnityEngine;

// --- NUEVO SCRIPT PARA ITEMS DE DADOS ---

[CreateAssetMenu(fileName = "NewDiceItem", menuName = "Inventory/Dice Item")]
public class DiceItem : ScriptableObject
{
    [Header("Dice Information")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Permanent Bonuses")]
    [Tooltip("How much to increase the player's base damage.")]
    public int damageBonus = 0;

    [Tooltip("How much to increase the player's max health (in quarter hearts).")]
    public int maxHealthBonus = 0;

    [Tooltip("How much to reduce the command failure chance (e.g., 0.05 for a 5% reduction).")]
    public float failureChanceReduction = 0f;
}
