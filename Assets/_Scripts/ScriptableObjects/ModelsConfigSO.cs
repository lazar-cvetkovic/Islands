using UnityEngine;

[CreateAssetMenu(fileName = "ModelsConfigSO", menuName = "ScriptableObjects/ModelsConfig")]
public class ModelsConfigSO : ScriptableObject
{
    [Header("Base Hex Tiles")]
    public GameObject WaterTile;
    public GameObject HeightFillTile;

    [Header("Height Hex Tiles")]
    public GameObject TileForHeightAbove0;
    public GameObject TileForHeightAbove100;
    public GameObject TileForHeightAbove200;
    public GameObject TileForHeightAbove300;
    public GameObject TileForHeightAbove400;
    public GameObject TileForHeightAbove500;
    public GameObject TileForHeightAbove600;
    public GameObject TileForHeightAbove700;
    public GameObject TileForHeightAbove800;
    public GameObject TileForHeightAbove900;
}
