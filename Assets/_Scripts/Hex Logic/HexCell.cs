using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    [SerializeField] private float _tileDropHeight = 3;

    public HexCoordinates Coordinates { get; private set; }
    public int Height { get; private set; }
    public int IslandId { get; set; } = -1;
    public bool IsWater => Height == 0;

    private ModelsConfigSO _modelsConfig;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

    public void Initialize(HexCoordinates coordinates, int height, ModelsConfigSO modelsConfig)
    {
        Coordinates = coordinates;
        Height = height;
        _modelsConfig = modelsConfig;

        SpawnTiles();

        _renderers = GetComponentsInChildren<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void SpawnTiles()
    {
        if (IsWater)
        {
            SpawnWater();
        }
        else
        {
            int fullHundreds = Height / 100;
            int remainder = Height % 100;

            int totalFillTiles = fullHundreds + (remainder > 0 ? 1 : 0);

            for (int i = 0; i < fullHundreds; i++)
            {
                SpawnFillTiles(_tileDropHeight, i);
            }

            if (remainder > 0)
            {
                SpawnAndScaleLastFillTile(fullHundreds, remainder, _tileDropHeight);
            }

            SpawnTopTile(totalFillTiles, _tileDropHeight);
            SpawnDecorations();
        }
    }

    private void SpawnWater() => Instantiate(_modelsConfig.WaterTile, transform.position, Quaternion.identity, transform);

    private void SpawnFillTiles(float dropHeight, int i)
    {
        Vector3 position = transform.position + Vector3.up * (dropHeight + i);
        GameObject fillTile = Instantiate(_modelsConfig.HeightFillTile, position, Quaternion.identity, transform);

        Rigidbody rb = fillTile.GetComponent<Rigidbody>() ?? fillTile.AddComponent<Rigidbody>();
        rb.mass = 1f;
    }

    private void SpawnAndScaleLastFillTile(int fullHundreds, int remainder, float dropHeight)
    {
        Vector3 position = transform.position + Vector3.up * (dropHeight + fullHundreds);
        GameObject fillTile = Instantiate(_modelsConfig.HeightFillTile, position, Quaternion.identity, transform);

        float scaleY = remainder / 100f;
        Vector3 localScale = fillTile.transform.localScale;
        localScale.y *= scaleY;
        fillTile.transform.localScale = localScale;

        Vector3 localPosition = fillTile.transform.localPosition;
        localPosition.y += (scaleY - 1f) / 2f;
        fillTile.transform.localPosition = localPosition;

        Rigidbody rb = fillTile.GetComponent<Rigidbody>() ?? fillTile.AddComponent<Rigidbody>();
        rb.mass = 1f;
    }

    private void SpawnTopTile(int totalFillTiles, float dropHeight)
    {
        Vector3 topPosition = transform.position + Vector3.up * (dropHeight + totalFillTiles);
        GameObject topObject = Instantiate(GetTopPrefab(), topPosition, Quaternion.identity, transform);
        Rigidbody topRb = topObject.GetComponent<Rigidbody>() ?? topObject.AddComponent<Rigidbody>();
        topRb.mass = 1f;
    }

    private GameObject GetTopPrefab()
    {
        int heightBracket = (Height / 100) * 100;

        return heightBracket switch
        {
            0 => _modelsConfig.TileForHeightAbove0,
            100 => _modelsConfig.TileForHeightAbove100,
            200 => _modelsConfig.TileForHeightAbove200,
            300 => _modelsConfig.TileForHeightAbove300,
            400 => _modelsConfig.TileForHeightAbove400,
            500 => _modelsConfig.TileForHeightAbove500,
            600 => _modelsConfig.TileForHeightAbove600,
            700 => _modelsConfig.TileForHeightAbove700,
            800 => _modelsConfig.TileForHeightAbove800,
            _ => _modelsConfig.TileForHeightAbove900,
        };
    }

    public void SpawnDecorations()
    {
        if (_modelsConfig.DecorationObjects == null || _modelsConfig.DecorationObjects.Decorations == null)
            return;

        var decorations = _modelsConfig.DecorationObjects.Decorations;

        int cellHeight = Height;

        var possibleDecorations = new List<DecorationObjectSO>();

        foreach (var decoration in decorations)
        {
            if (cellHeight >= decoration.MinHeight && cellHeight <= decoration.MaxHeight)
            {
                possibleDecorations.Add(decoration);
            }
        }

        if (possibleDecorations.Count == 0)
        {
            return;
        }

        float totalProbability = 0f;
        foreach (var decoration in possibleDecorations)
        {
            totalProbability += decoration.SpawnProbability;
        }

        if (totalProbability <= 0f)
        {
            return;
        }

        float randomValue = Random.Range(0f, totalProbability);

        float cumulativeProbability = 0f;

        DecorationObjectSO selectedDecoration = null;

        foreach (var decoration in possibleDecorations)
        {
            cumulativeProbability += decoration.SpawnProbability;

            if (randomValue <= cumulativeProbability)
            {
                selectedDecoration = decoration;
                break;
            }
        }

        if (selectedDecoration != null)
        {
            int spawnCount = 1;
            if (selectedDecoration.MaxSpawnPerTile > 1)
            {
                spawnCount = Random.Range(1, selectedDecoration.MaxSpawnPerTile + 1);
            }

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 decorationPosition = GetDecorationPosition(i, spawnCount);

                GameObject decorationObject = Instantiate(selectedDecoration.Prefab, decorationPosition, Quaternion.identity, transform);

                decorationObject.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
            }
        }
    }

    private Vector3 GetDecorationPosition(int index, int total)
    {
        Vector3 topPosition = GetTopTilePosition();

        if (total == 1)
        {
            return topPosition;
        }
        else
        {
            float angle = (360f / total) * index;
            float radius = HexMetrics.InnerRadius * 0.2f;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            return topPosition + new Vector3(x, 0f, z);
        }
    }

    private Vector3 GetTopTilePosition()
    {
        int fullHundreds = Height / 100;
        int remainder = Height % 100;
        int totalFillTiles = fullHundreds + (remainder > 0 ? 1 : 0);
        float dropHeight = _tileDropHeight;

        Vector3 topPosition = transform.position + Vector3.up * (dropHeight + totalFillTiles);

        return topPosition;
    }

    public void Highlight()
    {
        foreach (var renderer in _renderers)
        {
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, Color.gray);
            renderer.SetPropertyBlock(_propertyBlock);
        }

        AudioManager.Instance.PlaySFX(SoundType.Hover);
    }

    public void Unhighlight()
    {
        foreach (var renderer in _renderers)
        {
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, Color.white);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    public void SetHighlightColor(Color color)
    {
        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        foreach (var renderer in _renderers)
        {
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, color);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
