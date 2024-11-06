using System.Collections.Generic;
using System.Linq;

public class IslandDetector
{
    private HexCell[,] _cells;
    private int _width;
    private int _height;

    public Dictionary<int, List<HexCell>> Islands { get; private set; }
    public Dictionary<int, float> IslandAverageHeights { get; private set; }

    public IslandDetector(HexCell[,] cells, int width, int height)
    {
        _cells = cells;
        _width = width;
        _height = height;
        Islands = new Dictionary<int, List<HexCell>>();
    }

    public void DetectIslands()
    {
        int islandId = 0;
        bool[,] visited = new bool[_width, _height];

        for (int axialX = 0; axialX < _width; axialX++)
        {
            for (int axialY = 0; axialY < _height; axialY++)
            {
                if (!visited[axialX, axialY] && !_cells[axialX, axialY].IsWater)
                {
                    List<HexCell> islandCells = new List<HexCell>();
                    ExploreIsland(axialX, axialY, visited, islandCells, islandId);
                    Islands.Add(islandId, islandCells);
                    islandId++;
                }
            }
        }
    }

    private void ExploreIsland(int axialX, int axialY, bool[,] visited, List<HexCell> islandCells, int islandId)
    {
        Queue<HexCell> queue = new Queue<HexCell>();
        queue.Enqueue(_cells[axialX, axialY]);

        while (queue.Count > 0)
        {
            HexCell cell = queue.Dequeue();
            int x = cell.Coordinates.AxialX;
            int y = cell.Coordinates.AxialY;

            if (visited[x, y]) continue;

            visited[x, y] = true;
            cell.IslandId = islandId;
            islandCells.Add(cell);

            foreach (HexCell neighbor in GetNeighbors(x, y))
            {
                if (!visited[neighbor.Coordinates.AxialX, neighbor.Coordinates.AxialY] && !neighbor.IsWater)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private IEnumerable<HexCell> GetNeighbors(int axialX, int axialY)
    {
        int[][] directions = new int[][]
        {
            new int[]{ 1, 0 }, new int[]{ 0, 1 }, new int[]{ -1, 1 },
            new int[]{ -1, 0 }, new int[]{ 0, -1 }, new int[]{ 1, -1 }
        };

        foreach (var dir in directions)
        {
            int neighborX = axialX + dir[0];
            int neighborY = axialY + dir[1];

            if (neighborX >= 0 && neighborX < _width && neighborY >= 0 && neighborY < _height)
            {
                yield return _cells[neighborX, neighborY];
            }
        }
    }

    public void CalculateAverageHeights()
    {
        IslandAverageHeights = new Dictionary<int, float>();

        foreach (var island in Islands)
        {
            int totalHeight = island.Value.Sum(cell => cell.Height);
            float averageHeight = (float)totalHeight / island.Value.Count;
            IslandAverageHeights.Add(island.Key, averageHeight);
        }
    }

    public int GetHighestAverageIslandId() => IslandAverageHeights.OrderByDescending(i => i.Value).First().Key;

    public float GetAverageHeight(int islandId) => IslandAverageHeights.TryGetValue(islandId, out float averageHeight) ? averageHeight : 0f;
}
