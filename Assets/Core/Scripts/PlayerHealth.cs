using System;
using System.Collections;
using UnityEngine;

// --- CAMBIO SUGERIDO: Añadir referencia a PlayerStats ---
[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour
{
    // --- CONFIGURACIÓN EN EL INSPECTOR ---
    // [Header("Configuración de Vida")] // Comentado o eliminado, ya no es la fuente de verdad.
    // [Tooltip("El número de CONTENEDORES de corazón que tendrá el jugador.")]
    // public int maxHearts = 4; // Eliminado

    [Header("Referencias")]
    public HeartUI heartUI; // La UI de corazones del mundo (HUD)
    public HeartUI heartUIRPG; // La UI de corazones de la pantalla de batalla

    // --- VARIABLES PRIVADAS ---
    private int maxHealthInQuarters;
    public int currentHealth { get; private set; } // Modificado para acceso público de lectura
    private SpriteRenderer spriteRenderer;
    private PlayerStats playerStats; // Referencia al nuevo script

    // --- EVENTOS ---
    public static event Action OnPlayerDied;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerStats>(); // Obtenemos la referencia
    }

    void Start()
    {
        // Nos suscribimos al evento para cuando las estadísticas cambien
        playerStats.OnStatsChanged += UpdateHealthFromStats;
        // Inicializamos la vida por primera vez
        UpdateHealthFromStats();
    }

    private void OnDestroy()
    {
        // Es buena práctica desuscribirse de los eventos para evitar errores
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= UpdateHealthFromStats;
        }
    }

    /// <summary>
    /// Actualiza la vida máxima basándose en las estadísticas del PlayerStats.
    /// </summary>
    private void UpdateHealthFromStats()
    {
        maxHealthInQuarters = playerStats.maxHealth;
        int maxHearts = maxHealthInQuarters / 4;

        if (heartUI != null) heartUI.SetMaxHearts(maxHearts);
        if (heartUIRPG != null) heartUIRPG.SetMaxHearts(maxHearts);
        
        // Al actualizar la vida máxima, curamos al jugador completamente.
        // Podrías cambiar esta lógica si prefieres que la vida actual no se altere.
        ResetHealth();
    }

    // El resto del código de PlayerHealth permanece mayormente igual...

    // --- ELIMINADO: La lógica de colisión con el enemigo ahora está en Enemy.cs ---
    /*
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Esta lógica ha sido movida a Enemy.cs y cambiada a OnCollisionEnter2D
        // para un manejo de físicas más robusto.
    }
    */

    public void Heal(int amountInQuarters)
    {
        currentHealth += amountInQuarters;
        if (currentHealth > maxHealthInQuarters)
        {
            currentHealth = maxHealthInQuarters;
        }

        // --- CAMBIO SUGERIDO: Actualizar ambas UIs ---
        if (heartUI != null) heartUI.UpdateHearts(currentHealth);
        if (heartUIRPG != null) heartUIRPG.UpdateHearts(currentHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealthInQuarters;
        if (heartUI != null) heartUI.UpdateHearts(currentHealth);
        if (heartUIRPG != null) heartUIRPG.UpdateHearts(currentHealth);
    }

    public void TakeDamage(int damageInQuarters)
    {
        currentHealth -= damageInQuarters;
        if (currentHealth < 0) currentHealth = 0;

        // Actualizamos ambas UIs
        if (heartUI != null) heartUI.UpdateHearts(currentHealth);
        if (heartUIRPG != null) heartUIRPG.UpdateHearts(currentHealth);
        
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }
    
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    /// <summary>
    /// Actualiza temporalmente la UI de vida para la batalla.
    /// </summary>
    public void UpdateMaxHealthInBattle(int battleMaxHealth)
    {
        int maxHearts = battleMaxHealth / 4;
        if (heartUIRPG != null) heartUIRPG.SetMaxHearts(maxHearts);
        // Opcional: Curar al jugador al máximo de la nueva vida de batalla
        currentHealth = battleMaxHealth;
        if (heartUIRPG != null) heartUIRPG.UpdateHearts(currentHealth);
    }

    /// <summary>
    /// Restaura la UI de vida a los valores normales post-batalla.
    /// </summary>
    public void RestoreHealthAfterBattle()
    {
        UpdateHealthFromStats();
    }
}