using UnityEngine;
using UnityEngine.UI; // ¡Importante! Necesitamos esto para el componente Image

public enum Direction { Up, Right, Down, Left }

[RequireComponent(typeof(Image))]
public class RobotController : MonoBehaviour
{
    [Header("Estado del Robot")]
    public int currentX;
    public int currentY;
    public Direction currentDirection;

    [Header("Sprites Direccionales (Idle)")]
    public Sprite idleUpSprite;
    public Sprite idleRightSprite;
    public Sprite idleDownSprite;
    public Sprite idleLeftSprite;

    private LevelData currentLevelData;
    private Image robotImage;
    private Transform puzzleArea; // Referencia robusta al contenedor de tiles

    void Awake()
    {
        robotImage = GetComponent<Image>();
    }

    public void Setup(LevelData levelData, int startX, int startY, Transform puzzleArea)
    {
        this.currentLevelData = levelData;
        this.currentX = startX;
        this.currentY = startY;
        this.currentDirection = Direction.Down; // Siempre empieza mirando hacia arriba
        this.puzzleArea = puzzleArea; // Asignamos la referencia

        UpdateRobotVisuals(); // Actualiza la posición y el sprite inicial
    }

    public void TurnRight()
    {
        currentDirection = (Direction)(((int)currentDirection + 1) % 4);
        UpdateRobotVisuals(); // Actualizamos el sprite para que mire a la nueva dirección
    }

    public void TurnLeft()
    {
        currentDirection = (Direction)(((int)currentDirection - 1 + 4) % 4);
        UpdateRobotVisuals();
    }

    public void MoveForward()
    {
        int nextX = currentX;
        int nextY = currentY;

        switch (currentDirection)
        {
            case Direction.Up:    nextY--; break;
            case Direction.Right: nextX++; break;
            case Direction.Down:  nextY++; break;
            case Direction.Left:  nextX--; break;
        }

        if (IsPositionValid(nextX, nextY))
        {
            currentX = nextX;
            currentY = nextY;
            UpdateRobotVisuals(); // Movemos el robot a la nueva casilla
        }
        else
        {
            Debug.Log("¡Choque! Movimiento inválido.");
            // Aquí podrías añadir un pequeño efecto de sonido de "error" o "choque"
        }
    }

    // --- FUNCIÓN VISUAL ACTUALIZADA ---
    private void UpdateRobotVisuals()
    {
        if (robotImage == null)
        {
            robotImage = GetComponent<Image>();
        }

        // Ahora sí verificamos si falló de verdad
        if (robotImage == null)
        {
            Debug.LogError("RobotController: El componente 'Image' no se encontró...", gameObject);
            return; 
        }
        // 1. Cambiar el sprite manualmente según la dirección actual
        switch (currentDirection)
        {
            case Direction.Up:    robotImage.sprite = idleUpSprite;   break;
            case Direction.Right: robotImage.sprite = idleRightSprite; break;
            case Direction.Down:  robotImage.sprite = idleDownSprite;  break;
            case Direction.Left:  robotImage.sprite = idleLeftSprite;  break;
        }

        // 2. Mover el objeto al tile correcto (esta lógica no cambia)
        int newTileIndex = currentY * currentLevelData.Width + currentX;
        Transform newParentTile = puzzleArea.GetChild(newTileIndex);

        if (transform.parent != newParentTile)
        {
            transform.SetParent(newParentTile, false);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    private bool IsPositionValid(int x, int y)
    {
        if (x < 0 || x >= currentLevelData.Width || y < 0 || y >= currentLevelData.Height) return false;
        if (currentLevelData.rows[y].columns[x] == TileType.Wall) return false;
        return true;
    }
}