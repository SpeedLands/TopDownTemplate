using UnityEngine;
using System.Collections.Generic;

public enum TileType
{
    Empty,
    Wall,
    Start,
    Goal,
    Damage,
    Button,
    Spike
}

[System.Serializable]
public class GridRow
{
    // Quitamos el [9] fijo para que sea dinámico
    public TileType[] columns;
}

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Puzzle Metadata")]
    public string puzzleId;
    public int maxExecutions = 3;

    [Header("Allowed Commands")]
    public List<GameObject> allowedCommands;

    [Header("Grid Configuration")]
    [Tooltip("Tamaño del cuadrado (ej: 4 para 4x4, 5 para 5x5)")]
    [Range(4, 10)] // Limitamos el slider entre 4 y 10 para seguridad
    public int gridSize = 5; 

    // Ocultamos esto en el inspector si quieres, o lo dejamos para ver los datos
    public GridRow[] rows;

    // Propiedades automáticas
    public int Width => gridSize;
    public int Height => gridSize;

    // ESTA FUNCIÓN ES LA MAGIA
    // Se ejecuta cada vez que tocas algo en el Inspector de Unity
    private void OnValidate()
    {
        ValidateGridSize();
        EnforceWalls();
    }

    // 1. Asegura que el array tenga el tamaño correcto (NxN)
    private void ValidateGridSize()
    {
        // Si el array principal no existe o tiene tamaño incorrecto
        if (rows == null || rows.Length != gridSize)
        {
            System.Array.Resize(ref rows, gridSize);
        }

        // Recorremos cada fila
        for (int i = 0; i < gridSize; i++)
        {
            // Si la fila está vacía, la creamos
            if (rows[i] == null) rows[i] = new GridRow();

            // Si las columnas de esa fila no tienen el tamaño correcto
            if (rows[i].columns == null || rows[i].columns.Length != gridSize)
            {
                System.Array.Resize(ref rows[i].columns, gridSize);
            }
        }
    }

    // 2. Fuerza que los bordes sean siempre Muros
    private void EnforceWalls()
    {
        if (rows == null) return;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                // Condición de borde: Primera fila, Última fila, Primera columna, Última columna
                bool isBorder = (y == 0 || y == gridSize - 1 || x == 0 || x == gridSize - 1);

                if (isBorder)
                {
                    // Forzamos que sea muro
                    rows[y].columns[x] = TileType.Wall;
                }
            }
        }
    }
    
    // Botón opcional para limpiar el centro si quieres reiniciar
    [ContextMenu("Clear Inner Grid")]
    public void ClearCenter()
    {
        for (int y = 1; y < gridSize - 1; y++)
        {
            for (int x = 1; x < gridSize - 1; x++)
            {
                 rows[y].columns[x] = TileType.Empty;
            }
        }
        Debug.Log("Grid limpia (respetando muros)");
    }
}