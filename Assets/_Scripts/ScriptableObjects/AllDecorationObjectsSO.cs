using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "AllDecorationObjects", menuName = "ScriptableObjects/AllDecorationObjects", order = 2)]
public class AllDecorationObjectsSO : ScriptableObject
{
    public List<DecorationObjectSO> Decorations;

    public void SpawnDecorations(IHexCell cell)
    {
        if (Decorations == null || Decorations.Count == 0)
            return;

        int cellHeight = cell.Height;

        List<DecorationObjectSO> possibleDecorations = GetPossibleDecorations(cellHeight);

        if (possibleDecorations.Count == 0)
            return;

        float totalProbability = 0f;
        foreach (var decoration in possibleDecorations)
        {
            totalProbability += decoration.SpawnProbability;
        }

        if (totalProbability <= 0f)
            return;

        float randomValue = Random.Range(0f, totalProbability);

        DecorationObjectSO selectedDecoration = GetSelectedDecoration(possibleDecorations, randomValue);

        if (selectedDecoration == null)
        {
            return;
        }

        SpawnFinalDecorations(cell, cellHeight, selectedDecoration);
    }

    private List<DecorationObjectSO> GetPossibleDecorations(int cellHeight)
    {
        var possibleDecorations = new List<DecorationObjectSO>();
        foreach (var decoration in Decorations)
        {
            if (cellHeight >= decoration.MinHeight && cellHeight <= decoration.MaxHeight)
            {
                possibleDecorations.Add(decoration);
            }
        }

        return possibleDecorations;
    }

    private static DecorationObjectSO GetSelectedDecoration(List<DecorationObjectSO> possibleDecorations, float randomValue)
    {
        DecorationObjectSO selectedDecoration = null;
        float cumulativeProbability = 0f;

        foreach (var decoration in possibleDecorations)
        {
            cumulativeProbability += decoration.SpawnProbability;

            if (randomValue <= cumulativeProbability)
            {
                selectedDecoration = decoration;
                break;
            }
        }

        return selectedDecoration;
    }

    private void SpawnFinalDecorations(IHexCell cell, int cellHeight, DecorationObjectSO selectedDecoration)
    {
        int spawnCount = 1;
        if (selectedDecoration.MaxSpawnPerTile > 1)
        {
            spawnCount = Random.Range(1, selectedDecoration.MaxSpawnPerTile + 1);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 decorationPosition = GetDecorationPosition(cell, i, spawnCount);

            if (selectedDecoration.Prefab == null) return;

            GameObject decorationObject = Instantiate(selectedDecoration.Prefab, decorationPosition, Quaternion.identity, cell.CellTransform);
            decorationObject.transform.Rotate(0f, Random.Range(0f, 360f), 0f);

            if (cellHeight != 0)
            {
                HexCell.AddRigidbody(decorationObject);
            }
        }
    }

    private Vector3 GetDecorationPosition(IHexCell cell, int index, int total)
    {
        Vector3 basePosition = cell.GetTopTilePosition();

        if (total == 1)
        {
            return basePosition;
        }
        else
        {
            float angle = (360f / total) * index;
            float radius = HexMetrics.InnerRadius * 0.2f;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            return basePosition + new Vector3(x, 0f, z);
        }
    }
}
