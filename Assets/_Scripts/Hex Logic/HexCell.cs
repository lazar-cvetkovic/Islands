using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour, IHexCell
{
    public const RigidbodyConstraints CellRigidbodyConstraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
                                                                  RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    
    [SerializeField] private float _tileDropHeight = 3;

    public HexCoordinates Coordinates { get; private set; }
    public int Height { get; private set; }
    public int IslandId { get; set; } = -1;
    public bool IsWater => Height == 0;
    public Transform CellTransform => transform;

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
                SpawnFillTile(i);
            }

            if (remainder > 0)
            {
                SpawnAndScaleLastFillTile(fullHundreds, remainder);
            }

            SpawnTopTile(totalFillTiles);
        }

        if (_modelsConfig.DecorationObjects != null)
        {
            _modelsConfig.DecorationObjects.SpawnDecorations(this);
        }
    }

    private void SpawnWater()
    {
        GameObject waterTile = Instantiate(_modelsConfig.WaterTile, transform.position, Quaternion.identity, transform);

        AddRigidbody(waterTile);
    }

    public static Rigidbody AddRigidbody(GameObject fillTile)
    {
        Rigidbody rb = fillTile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = fillTile.AddComponent<Rigidbody>();
        }

        rb.mass = 1f;
        rb.constraints = CellRigidbodyConstraints;

        return rb;
    }

    private void SpawnFillTile(int index)
    {
        Vector3 position = transform.position + Vector3.up * (_tileDropHeight + index);
        GameObject fillTile = Instantiate(_modelsConfig.HeightFillTile, position, Quaternion.identity, transform);

        AddRigidbody(fillTile);
    }


    private void SpawnAndScaleLastFillTile(int index, int remainder)
    {
        Vector3 position = transform.position + Vector3.up * (_tileDropHeight + index);
        GameObject fillTile = Instantiate(_modelsConfig.HeightFillTile, position, Quaternion.identity, transform);

        float scaleY = remainder / 100f;
        Vector3 localScale = fillTile.transform.localScale;
        localScale.y *= scaleY;
        fillTile.transform.localScale = localScale;

        Vector3 localPosition = fillTile.transform.localPosition;
        localPosition.y += (scaleY - 1f) / 2f;
        fillTile.transform.localPosition = localPosition;

        AddRigidbody(fillTile);
    }

    private void SpawnTopTile(int totalFillTiles)
    {
        Vector3 topPosition = transform.position + Vector3.up * (_tileDropHeight + totalFillTiles);
        GameObject topObject = Instantiate(GetTopPrefab(), topPosition, Quaternion.identity, transform);

        Rigidbody topRb = AddRigidbody(topObject);
        topRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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
