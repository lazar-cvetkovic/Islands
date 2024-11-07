using UnityEngine;

[CreateAssetMenu(fileName = "DecorationObject", menuName = "ScriptableObjects/DecorationObject", order = 1)]
public class DecorationObjectSO : ScriptableObject
{
    public GameObject Prefab;

    [Tooltip("Minimum height (inclusive) where this decoration can spawn.")]
    [Range(0f, 1000f)]
    public int MinHeight;

    [Tooltip("Maximum height (inclusive) where this decoration can spawn.")]
    [Range(0f, 1000f)]
    public int MaxHeight;

    [Tooltip("Maximum number of this decoration to spawn per tile.")]
    public int MaxSpawnPerTile = 1;

    [Tooltip("Probability weight for this decoration to be chosen.")]
    [Range(0f, 1f)]
    public float SpawnProbability;
}
