using System.Collections;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Configuración de Waypoints")]
    public Transform waypointParent; // Arrastrar el objeto padre de los waypoints aquí
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    // Variables privadas
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    // Referencias a componentes
    private Animator animator;

    // Variables para la animación
    private Vector2 lastDirection = Vector2.down; // Dirección por defecto al estar quieto

    // Awake se usa para obtener componentes
    void Awake()
    {
        // --- CORRECCIÓN CRÍTICA #1: Asignar el Animator ---
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // --- CORRECCIÓN CRÍTICA #2: Añadir comprobaciones de seguridad ---
        if (waypointParent == null)
        {
            Debug.LogError("¡No se ha asignado un 'waypointParent'! El NPC no se moverá.", this);
            this.enabled = false; // Desactivamos el script para evitar más errores
            return;
        }

        if (waypointParent.childCount == 0)
        {
            Debug.LogError("¡El 'waypointParent' no tiene hijos (waypoints)! El NPC no se moverá.", this);
            this.enabled = false;
            return;
        }

        // Llenamos el array de waypoints
        waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        // Si el juego está pausado, no hacemos nada
        if (PauseController.IsGamePaused)
        {
            UpdateAnimation(Vector2.zero); // Actualizamos la animación a estado idle
            return;
        }

        // Si no estamos esperando, nos movemos
        if (!isWaiting)
        {
            MoveToWaypoint();
        }
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
        
        // Calculamos la dirección hacia el objetivo
        Vector2 direction = (target.position - transform.position).normalized;

        // Movemos al personaje
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Actualizamos la animación con la dirección actual
        UpdateAnimation(direction);

        // Comprobamos si hemos llegado al destino
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    // --- LÓGICA DE ANIMACIÓN CENTRALIZADA ---
    void UpdateAnimation(Vector2 direction)
    {
        bool isWalking = direction.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            // Si nos movemos, actualizamos la dirección del blend tree
            animator.SetFloat("InputX", direction.x);
            animator.SetFloat("InputY", direction.y);
            lastDirection = direction; // Guardamos la última dirección de movimiento
        }
        else
        {
            // Si estamos quietos, usamos la última dirección para la animación de idle
            animator.SetFloat("InputX", lastDirection.x);
            animator.SetFloat("InputY", lastDirection.y);
            animator.SetFloat("LastInputX", lastDirection.x);
            animator.SetFloat("LastInputY", lastDirection.y);
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        UpdateAnimation(Vector2.zero); // Ponemos la animación en idle mientras esperamos
        
        yield return new WaitForSeconds(waitTime);

        // Calculamos el siguiente waypoint
        if (loopWaypoints)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            // Se detiene en el último waypoint
            currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);
        }

        isWaiting = false;
    }
}