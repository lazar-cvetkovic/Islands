using UnityEngine;
using System.Collections.Generic;

public class HexCellOptimized : MonoBehaviour, IHexCell
{
    public HexCoordinates Coordinates { get; private set; }
    public int Height { get; private set; }
    public int IslandId { get; set; } = -1;
    public bool IsWater => Height == 0;
    public Transform CellTransform => transform;

    private ModelsConfigSO _modelsConfig;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

    private List<GameObject> _spawnedTiles = new List<GameObject>();

    private const float InitialLocalYPosition = -1.5f;
    private const float TileHeight = 0.15f;

    public void Initialize(HexCoordinates coordinates, int height, ModelsConfigSO modelsConfig)
    {
        Coordinates = coordinates;
        Height = height;
        _modelsConfig = modelsConfig;

        SpawnTiles();

        _renderers = GetComponentsInChildren<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();

        SetHighlightColor(Color.white);
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

            float currentHeight = 0f;

            for (int i = 0; i < fullHundreds; i++)
            {
                SpawnFillTile(i);
                currentHeight += TileHeight;
            }

            if (remainder > 0)
            {
                float scaledTileHeight = SpawnScaledFillTile(fullHundreds, remainder);
                currentHeight += scaledTileHeight;
            }

            SpawnTopTile(currentHeight);
        }

        if (_modelsConfig.DecorationObjects != null)
        {
            _modelsConfig.DecorationObjects.SpawnDecorations(this);
        }
    }

    private void SpawnWater()
    {
        GameObject waterTile = ObjectPooler.Instance.GetPooledObject(_modelsConfig.WaterTile);
        waterTile.transform.SetParent(transform);
        waterTile.transform.localPosition = new Vector3(0f, InitialLocalYPosition, 0f);
        waterTile.transform.localRotation = Quaternion.identity;
        waterTile.SetActive(true);

        _spawnedTiles.Add(waterTile);
    }

    private void SpawnFillTile(int index)
    {
        GameObject fillTile = ObjectPooler.Instance.GetPooledObject(_modelsConfig.HeightFillTile);
        fillTile.transform.SetParent(transform);

        float localYPosition = InitialLocalYPosition + index * TileHeight;
        fillTile.transform.localPosition = new Vector3(0f, localYPosition, 0f);
        fillTile.transform.localRotation = Quaternion.identity;
        fillTile.SetActive(true);

        _spawnedTiles.Add(fillTile);
    }

    private float SpawnScaledFillTile(int index, int remainder)
    {
        GameObject fillTile = ObjectPooler.Instance.GetPooledObject(_modelsConfig.HeightFillTile);
        fillTile.transform.SetParent(transform);
        float scaleY = ScaleTile(remainder, fillTile);

        float adjustedTileHeight = TileHeight * scaleY;
        float localYPosition = InitialLocalYPosition + index * TileHeight + (adjustedTileHeight - TileHeight) / 2f;

        fillTile.transform.localPosition = new Vector3(0f, localYPosition, 0f);
        fillTile.transform.localRotation = Quaternion.identity;
        fillTile.SetActive(true);

        _spawnedTiles.Add(fillTile);

        return adjustedTileHeight;
    }

    private float ScaleTile(int remainder, GameObject fillTile)
    {
        float scaleY = remainder / 100f;
        Vector3 localScale = fillTile.transform.localScale;
        localScale.y *= scaleY;
        fillTile.transform.localScale = localScale;

        return scaleY;
    }

    private void SpawnTopTile(float currentHeight)
    {
        GameObject topTilePrefab = GetTopPrefab();
        GameObject topTile = ObjectPooler.Instance.GetPooledObject(topTilePrefab);
        topTile.transform.SetParent(transform);

        float localYPosition = InitialLocalYPosition + currentHeight;
        topTile.transform.localPosition = new Vector3(0f, localYPosition, 0f);
        topTile.transform.localRotation = Quaternion.identity;
        topTile.SetActive(true);

        _spawnedTiles.Add(topTile);
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

    public Vector3 GetTopTilePosition()
    {
        float currentHeight = 0f;
        int fullHundreds = Height / 100;
        int remainder = Height % 100;

        currentHeight += fullHundreds * TileHeight;

        if (remainder > 0)
        {
            float scaleY = remainder / 100f;
            float scaledTileHeight = TileHeight * scaleY;
            currentHeight += scaledTileHeight;
        }

        float localYPosition = InitialLocalYPosition + currentHeight;

        Vector3 topPosition = transform.position + Vector3.up * localYPosition;
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

    private void OnDestroy()
    {
        foreach (var tile in _spawnedTiles)
        {
            tile.SetActive(false);
            ObjectPooler.Instance?.ReturnPooledObject(tile);
        }
        _spawnedTiles.Clear();
    }
}
