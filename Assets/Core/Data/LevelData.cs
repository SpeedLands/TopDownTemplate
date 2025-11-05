using UnityEngine;

// Enum para definir qué tipo de tile es cada celda de la cuadrícula
public enum TileType
{
    Empty,
    Wall,
    Start,
    Goal,
    Damage,
    Button,
}

[System.Serializable]
public class GridRow
{
    // Cada fila tiene un array de columnas
    public TileType[] columns = new TileType[9]; 
}


[CreateAssetMenu(fileName = "New Level", menuName = "Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    public int Width => rows.Length > 0 ? rows[0].columns.Length : 0;
    public int Height => rows.Length;

    [Header("Grid Layout Visual")]
    public GridRow[] rows = new GridRow[9]; 
}