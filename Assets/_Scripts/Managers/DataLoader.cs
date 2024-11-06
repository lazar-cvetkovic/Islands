using System;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    public int[,] HeightMap { get; private set; }

    public void LoadData(Action<bool> onComplete)
    {
        BackendManager.Instance.LoadGameData((data) =>
        {
            if (!string.IsNullOrEmpty(data))
            {
                ParseData(data);
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError("Failed to load data from backend.");
                onComplete?.Invoke(false);
            }
        });
    }

    private void ParseData(string data)
    {
        string[] rows = data.Trim().Split('\n');
        int size = rows.Length;
        HeightMap = new int[size, size];

        for (int y = 0; y < size; y++)
        {
            string[] cells = rows[y].Trim().Split(' ');
            for (int x = 0; x < cells.Length; x++)
            {
                HeightMap[x, y] = int.Parse(cells[x]);
            }
        }
    }
}
