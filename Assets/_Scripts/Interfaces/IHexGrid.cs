using UnityEngine;

public interface IHexGrid
{
    GameObject CellPrefab { get; }
    IHexCell[,] Cells { get; }
    int Height { get; }
    int Width { get; }

    void ClearGrid();
    void GenerateGrid(int[,] heightMap);
}
