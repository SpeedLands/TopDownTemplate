using UnityEngine;
using UnityEngine.UI; // Necesario para Image y GridLayoutGroup

public class GridGenerator : MonoBehaviour
{
    [Header("Prefabs y Sprites")]
    public GameObject tilePrefab;    // El prefab de una casilla (una UI Image).
    public GameObject robotPrefab;   // El prefab del robot (otra UI Image).

    // Asigna aquí los sprites individuales desde tu spritesheet en el Inspector
    public Sprite emptySprite;
    public Sprite wallSprite;
    public Sprite startSprite;
    public Sprite goalSprite;
    public Sprite damageSprite;
    public Sprite buttonSprite;

    private GameObject robotInstance; // Para guardar una referencia al robot creado
    public LevelData levelDataForWinCheck;

    // Esta es la función principal que tu script Puzzle.cs llamará
    public void GenerateGrid(LevelData levelData)
    {
        levelDataForWinCheck = levelData;

        // 2. Configurar el componente GridLayoutGroup
        // Este componente ordena automáticamente los hijos en una cuadrícula.
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        if (gridLayout == null) // Si no lo tiene, lo añadimos
        {
            gridLayout = gameObject.AddComponent<GridLayoutGroup>();
        }
        // Le decimos que el número de columnas es fijo y es igual al ancho de nuestro nivel.
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = levelData.Width; // Usamos la propiedad Width del LevelData mejorado

        // 3. Crear cada tile del nivel usando dos bucles (para filas y columnas)
        for (int y = 0; y < levelData.Height; y++) // Bucle para las filas (y)
        {
            for (int x = 0; x < levelData.Width; x++) // Bucle para las columnas (x)
            {
                // Creamos un nuevo tile usando el prefab y lo hacemos hijo de este objeto (PuzzleArea)
                GameObject newTile = Instantiate(tilePrefab, transform);
                newTile.name = $"Tile_{x}_{y}";

                Image tileImage = newTile.GetComponent<Image>();
                // Accedemos al tipo de tile usando las coordenadas [y] y [x] de la estructura de datos
                TileType currentType = levelData.rows[y].columns[x];
                
                // Asignamos el sprite correcto según el tipo de tile
                tileImage.sprite = GetSpriteForTileType(currentType);

                // Si es el tile de inicio, creamos el robot encima de él
                if (currentType == TileType.Start)
                {
                    // Instanciamos el robot como hijo del tile de inicio para que se posicione correctamente
                    robotInstance = Instantiate(robotPrefab, newTile.transform);
                    robotInstance.name = "Robot";
                    RobotController robotController = robotInstance.GetComponent<RobotController>();
                    robotController.Setup(levelData, x, y);
                }
            }
        }
    }

    // Función de ayuda para obtener el sprite correcto. Mantiene el código principal más limpio.
    private Sprite GetSpriteForTileType(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Wall:
                return wallSprite;
            case TileType.Start:
                return startSprite;
            case TileType.Goal:
                return goalSprite;
            case TileType.Damage:
                return damageSprite;
            case TileType.Button:
                return buttonSprite;
            case TileType.Empty:
            default:
                return emptySprite;
        }
    }

    public void ClearGrid()
    {
        // Limpiamos todos los tiles que son hijos de este objeto (PuzzleArea)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Nos aseguramos de que la referencia al robot también se limpie
        if (robotInstance != null)
        {
            Destroy(robotInstance);
            robotInstance = null; // ¡Importante! Ponemos la variable a null de verdad.
        }
    }
}