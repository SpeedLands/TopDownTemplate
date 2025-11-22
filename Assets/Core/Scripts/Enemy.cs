using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El objetivo que este enemigo perseguirá. Asigna el jugador aquí.")]
    [SerializeField] private Transform playerTransform;

    [Header("Parámetros de Movimiento")]
    [Tooltip("La velocidad máxima del enemigo.")]
    [SerializeField] private float chaseSpeed = 3f;
    [Tooltip("La distancia a la que el enemigo se detendrá del jugador.")]
    [SerializeField] private float stoppingDistance = 1.5f;

    // Referencias a componentes cacheadas.
    private Rigidbody2D rb;
    private Animator animator;

    // Variables de estado para la animación.
    private Vector2 moveDirection;
    private Vector2 lastMoveDirection; // Guardamos la última dirección para el estado de reposo (idle).

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("PlayerTransform no está asignado en el enemigo: " + gameObject.name + ". El script se desactivará.");
            enabled = false;
        }
    }

    // Update es para la lógica visual (animación).
    void Update()
    {
        UpdateAnimator();
    }

    // FixedUpdate es para la lógica de físicas (movimiento).
    void FixedUpdate()
    {
        if (playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 directionToPlayer = (playerTransform.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > stoppingDistance)
        {
            // Nos estamos moviendo.
            moveDirection = directionToPlayer.normalized;
            rb.linearVelocity = moveDirection * chaseSpeed;

            // Guardamos la última dirección en la que nos movimos.
            lastMoveDirection = moveDirection;
        }
        else
        {
            // Estamos parados.
            moveDirection = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// Actualiza el Animator basándose en la dirección de movimiento.
    private void UpdateAnimator()
    {
        // El enemigo está caminando si su dirección de movimiento no es cero.
        bool isWalking = moveDirection.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            // Si se mueve, pasamos la dirección actual al Blend Tree.
            animator.SetFloat("InputX", moveDirection.x);
            animator.SetFloat("InputY", moveDirection.y);
        }
        else
        {
            // Si está parado, usamos la ÚLTIMA dirección para que se quede mirando
            // hacia donde iba. Esto evita que el sprite vuelva a una posición por defecto.
            animator.SetFloat("InputX", lastMoveDirection.x);
            animator.SetFloat("InputY", lastMoveDirection.y);
            animator.SetFloat("LastInputX", lastMoveDirection.x);
            animator.SetFloat("LastInputY", lastMoveDirection.y);
        }
    }

    // --- NUEVO: Iniciar batalla al colisionar con el jugador ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos si el objeto con el que colisionamos tiene el componente PlayerHealth.
        if (collision.gameObject.GetComponent<PlayerHealth>() != null)
        {
            // Si el BattleManager existe, le decimos que inicie la batalla.
            if (BattleManager.Instance != null)
            {
                Debug.Log("Player collided with enemy. Starting battle.");
                BattleManager.Instance.StartBattle(this);
            }
            else
            {
                Debug.LogError("BattleManager.Instance is not found in the scene!");
            }
        }
    }
}