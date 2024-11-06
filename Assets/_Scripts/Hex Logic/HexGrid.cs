using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int Width = 30;
    public int Height = 30;
    public GameObject HexCellPrefab;
    public ModelsConfigSO ModelsConfig;

    private HexCell[,] _cells;
    public HexCell[,] Cells => _cells;

    public void GenerateGrid(int[,] heightMap)
    {
        _cells = new HexCell[Width, Height];
        for (int axialX = 0; axialX < Width; axialX++)
        {
            for (int axialY = 0; axialY < Height; axialY++)
            {
                CreateCell(axialX, axialY, heightMap[axialX, axialY]);
            }
        }
    }

    private void CreateCell(int axialX, int axialY, int cellHeight)
    {
        Vector3 position = CalculatePosition(axialX, axialY);
        GameObject cellObject = Instantiate(HexCellPrefab, position, Quaternion.identity, transform);
        HexCell cell = cellObject.GetComponent<HexCell>();
        cell.Initialize(new HexCoordinates(axialX, axialY), cellHeight, ModelsConfig);
        _cells[axialX, axialY] = cell;
    }

    private Vector3 CalculatePosition(int axialX, int axialY)
    {
        float x = HexMetrics.InnerRadius * (Mathf.Sqrt(3f) * axialX + Mathf.Sqrt(3f) / 2f * axialY);
        float z = HexMetrics.InnerRadius * (1.5f * axialY);
        return new Vector3(x, 0f, z);
    }
}