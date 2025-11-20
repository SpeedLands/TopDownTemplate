using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    // Referencias a componentes
    private Rigidbody2D rb;
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;
    public MenuController menuController; // Asignar en el Inspector

    // Variables de estado
    private Vector2 moveInput;
    private Vector2 lastMoveDirection;

    // Propiedad para centralizar las condiciones de movimiento
    private bool CanMove => !PauseController.IsGamePaused && (menuController == null || !menuController.IsMenuOpen());

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // playerInput no necesita ser una variable de clase si solo se usa para los eventos
    }

    // Update es para lógica visual y no física
    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            if (rb.linearVelocity != Vector2.zero)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetFloat("LastInputX", lastMoveDirection.x);
                animator.SetFloat("LastInputY", lastMoveDirection.y);
            }
            return;
        }
        UpdateAnimator();
    }

    // FixedUpdate es para toda la lógica de física
    void FixedUpdate()
    {
        if (CanMove)
        {
            // Aplicamos el movimiento usando la variable que actualiza OnMove
            rb.linearVelocity = moveInput * moveSpeed; 
        }
        else
        {
            // Si no nos podemos mover, detenemos al personaje
            rb.linearVelocity = Vector2.zero;
        }
    }

    // OnMove solo se encarga de leer el input
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput.normalized; // Normalizamos para tener una dirección pura (longitud 1)
        }
    }

    public void OnToggleMenu(InputAction.CallbackContext context)
    {
        if (context.performed && menuController != null)
        {
            menuController.ToggleMenu();
        }
    }

    // Método dedicado para actualizar el Animator
    private void UpdateAnimator()
    {
        // El personaje está caminando si se puede mover y el input no es cero
        bool isWalking = CanMove && moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalking", isWalking);

        // Actualizamos la dirección del blend tree solo si nos estamos moviendo
        if (isWalking)
        {
            if (!playingFootsteps)
            {
                StartFootsteps();
            }

            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
        // Si no nos movemos, usamos la última dirección guardada para el idle
        else
        {
            if (playingFootsteps)
            {
                StopFootsteps();
            }
            // Esto asegura que el personaje mire en la última dirección al quedarse quieto
            animator.SetFloat("InputX", lastMoveDirection.x);
            animator.SetFloat("InputY", lastMoveDirection.y);
            
            animator.SetFloat("LastInputX", lastMoveDirection.x);
            animator.SetFloat("LastInputY", lastMoveDirection.y);
        }
    }

    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootseps), 0f, footstepSpeed);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootseps));
    }

    void PlayFootseps()
    {
        SoundEffectManager.Play("Footstep", true);
    }
}