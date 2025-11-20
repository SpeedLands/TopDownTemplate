using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // --- CONFIGURACIÓN EN EL INSPECTOR ---
    [Header("Configuración de Vida")]
    [Tooltip("El número de CONTENEDORES de corazón que tendrá el jugador.")]
    public int maxHearts = 4;

    [Header("Referencias")]
    public HeartUI heartUI; // La UI de corazones del mundo (HUD)
    public HeartUI heartUIRPG; // La UI de corazones de la pantalla de batalla

    // --- VARIABLES PRIVADAS ---
    private int maxHealthInQuarters;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    // --- EVENTOS ---
    public static event Action OnPlayerDied;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxHealthInQuarters = maxHearts * 4;

        // Inicializamos ambas UIs de corazones
        if (heartUI != null) heartUI.SetMaxHearts(maxHearts);
        if (heartUIRPG != null) heartUIRPG.SetMaxHearts(maxHearts);
        
        ResetHealth();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // --- CAMBIO SUGERIDO: Lógica de inicio de batalla más limpia ---
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            // El trabajo de este script es solo notificar. El BattleManager hará el resto.
            BattleManager.Instance.StartBattle(enemy);
        }
        
        // Puedes mantener tu lógica para las trampas aquí si lo deseas
        // Trap trap = collision.GetComponent<Trap>();
        // if (trap) { ... }
    }

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
}