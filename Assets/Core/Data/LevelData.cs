using UnityEngine;
using System.Collections.Generic;

// Enum para definir qué tipo de tile es cada celda de la cuadrícula
public enum TileType
{
    Empty,
    Wall,
    Start,
    Goal,
    Damage,
    Button,
    Spike // Nuevo tipo de tile para los picos
}

[System.Serializable]
public class GridRow
{
    // Cada fila tiene un array de columnas
    public TileType[] columns = new TileType[9];
}


[CreateAssetMenu(fileName = "NewLevelData", menuName = "Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Puzzle Metadata")]
    [Tooltip("ID única para identificar este puzzle. Usada por el sistema de eventos.")]
    public string puzzleId;

    [Tooltip("Número máximo de veces que el jugador puede ejecutar la secuencia.")]
    public int maxExecutions = 3;

    [Header("Allowed Commands")]
    [Tooltip("Lista de prefabs de comandos de UI permitidos en la paleta para este nivel.")]
    public List<GameObject> allowedCommands;

    [Header("Grid Configuration")]
    public GridRow[] rows = new GridRow[9];

    // Propiedades de conveniencia para obtener las dimensiones
    public int Width => rows.Length > 0 ? rows[0].columns.Length : 0;
    public int Height => rows.Length;
}
