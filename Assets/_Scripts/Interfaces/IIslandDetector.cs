using System.Collections.Generic;

public interface IIslandDetector
{
    Dictionary<int, float> IslandAverageHeights { get; }
    Dictionary<int, List<IHexCell>> Islands { get; }

    void CalculateAverageHeights();
    void DetectIslands();
    float GetAverageHeight(int islandId);
    int GetHighestAverageIslandId();
}
