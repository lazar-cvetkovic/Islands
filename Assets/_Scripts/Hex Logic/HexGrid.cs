using UnityEngine;

public class HexGrid : MonoBehaviour, IHexGrid
{
    [SerializeField] private int _width = 30;
    [SerializeField] private int _height = 30;

    [SerializeField] private GameObject _optimizedCellPrefab;
    [SerializeField] private GameObject _hexCellPrefab;

    [SerializeField] private ModelsConfigSO _modelsConfig;

    private IHexCell[,] _cells;

    public IHexCell[,] Cells => _cells;
    public int Height => _height;
    public int Width => _width;
    public GameObject CellPrefab => GameManager.Instance.IsOptimized ? _optimizedCellPrefab : _hexCellPrefab;

    public void GenerateGrid(int[,] heightMap)
    {
        _cells = new IHexCell[_width, _height];
        for (int axialX = 0; axialX < _width; axialX++)
        {
            for (int axialY = 0; axialY < _height; axialY++)
            {
                CreateCell(axialX, axialY, heightMap[axialX, axialY]);
            }
        }
    }

    public void ClearGrid()
    {
        if (_cells != null)
        {
            foreach (var cell in _cells)
            {
                if (cell != null)
                {
                    MonoBehaviour cellMono = cell as MonoBehaviour;
                    if (cellMono != null)
                    {
                        Destroy(cellMono.gameObject);
                    }
                }
            }
        }
    }

    private void CreateCell(int axialX, int axialY, int cellHeight)
    {
        Vector3 position = CalculatePosition(axialX, axialY);
        GameObject cellObject = Instantiate(CellPrefab, position, Quaternion.identity, transform);
        IHexCell cell = cellObject.GetComponent<IHexCell>();
        cell.Initialize(new HexCoordinates(axialX, axialY), cellHeight, _modelsConfig);
        _cells[axialX, axialY] = cell;
    }

    private Vector3 CalculatePosition(int axialX, int axialY)
    {
        float x = HexMetrics.InnerRadius * (Mathf.Sqrt(3f) * axialX + Mathf.Sqrt(3f) / 2f * axialY);
        float z = HexMetrics.InnerRadius * (1.5f * axialY);
        return new Vector3(x, 0f, z);
    }
}
