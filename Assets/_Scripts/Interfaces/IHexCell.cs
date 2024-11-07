using UnityEngine;

public interface IHexCell
{
    HexCoordinates Coordinates { get; }
    int Height { get; }
    int IslandId { get; set; }
    bool IsWater { get; }
    Transform CellTransform { get; }

    Vector3 GetTopTilePosition();

    void Initialize(HexCoordinates coordinates, int height, ModelsConfigSO modelsConfig);
    void SetHighlightColor(Color color);
    void Highlight();
    void Unhighlight();
}