// --- NUEVA CLASE PARA ESTADÍSTICAS TEMPORALES EN BATALLA ---
// No es un MonoBehaviour, por lo que podemos crear instancias libremente.

public class PlayerBattleStats
{
    public int level;
    public int programmingSkill;
    public int maxHealth;
    public int baseDamage;
    public float failureChanceReduction = 0f; // Para acumular bonus de dados

    /// <summary>
    /// Constructor que copia las estadísticas de un PlayerStats (MonoBehaviour).
    /// </summary>
    public PlayerBattleStats(PlayerStats source)
    {
        this.level = source.level;
        this.programmingSkill = source.programmingSkill;
        this.maxHealth = source.maxHealth;
        this.baseDamage = source.baseDamage;
    }

    /// <summary>
    /// Calcula la probabilidad de fallo del comando, incluyendo las reducciones de los dados.
    /// </summary>
    public float GetCommandFailureChance()
    {
        float baseChance = UnityEngine.Mathf.Max(0.05f, 0.25f - (programmingSkill * 0.02f));
        return UnityEngine.Mathf.Max(0.05f, baseChance - failureChanceReduction);
    }
}
