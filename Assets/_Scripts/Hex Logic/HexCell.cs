using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexCell : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.HandleCellClick(this);
    }

    public void Highlight()
    {
        foreach (var renderer in _renderers)
        {
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, Color.gray); 
            renderer.SetPropertyBlock(_propertyBlock);
        }
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
