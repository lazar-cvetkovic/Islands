using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;

    private Vector3 _direction;
    private Quaternion _targetRotation;

    private float _changeDirectionInterval = 5f;
    private float _changeDirectionTimer;

    private HexGrid _hexGrid;

    private Vector3 _basePosition;
    private bool _shouldBoatMove = false;

    private const float _startingYPosition = -1.4f;

    private void Awake()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, _startingYPosition, transform.localPosition.z);
    }

    private IEnumerator Start()
    {
        InitializeData();

        yield return new WaitForSeconds(2);
        _shouldBoatMove = true;
    }

    private void InitializeData()
    {
        _hexGrid = FindFirstObjectByType<HexGrid>();

        if (!FindRandomWaterPosition(out _basePosition))
        {
            Debug.LogError("No water tiles found in the grid!");
            enabled = false;
            return;
        }

        _basePosition.y = _startingYPosition;
        transform.position = _basePosition;

        PickInitialRandomDirection();

        _changeDirectionTimer = _changeDirectionInterval;

        if (_direction != Vector3.zero)
        {
            _targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = _targetRotation;
        }
    }

    private void Update()
    {
        if (!_shouldBoatMove) return;

        MoveBoat();
        UpdateDirectionTimer();
    }

    private void UpdateDirectionTimer()
    {
        _changeDirectionTimer -= Time.deltaTime;
        if (_changeDirectionTimer <= 0f)
        {
            PickRandomDirection();
            _changeDirectionTimer = _changeDirectionInterval;
        }
    }

    private void MoveBoat()
    {
        Vector3 movement = _direction * _speed * Time.deltaTime;

        Vector3 nextBasePosition = _basePosition + movement;

        if (!IsPositionOverWater(nextBasePosition))
        {
            PickRandomDirection();
            return;
        }

        _basePosition = nextBasePosition;

        transform.position = new Vector3(_basePosition.x, _startingYPosition, _basePosition.z);

        if (_direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 2f);
        }
    }

    private void PickInitialRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        _direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        _targetRotation = Quaternion.LookRotation(_direction);
    }

    private void PickRandomDirection()
    {
        int attempts = 10;
        for (int i = 0; i < attempts; i++)
        {
            float angle = Random.Range(-45f, 45f);
            Vector3 newDirection = Quaternion.Euler(0, angle, 0) * _direction;
            newDirection.Normalize();

            Vector3 predictedPosition = _basePosition + newDirection * _speed * Time.deltaTime * 5f;

            if (IsPositionOverWater(predictedPosition))
            {
                _direction = newDirection;
                _targetRotation = Quaternion.LookRotation(_direction);
                return;
            }
        }

        _direction = -_direction;
        _targetRotation = Quaternion.LookRotation(_direction);
    }

    private bool IsPositionOverWater(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 10f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            HexCell cell = hit.collider.GetComponentInParent<HexCell>();
            if (cell != null)
            {
                return cell.IsWater;
            }
        }
        return false;
    }

    private bool FindRandomWaterPosition(out Vector3 position)
    {
        var waterCells = new List<IHexCell>();
        foreach (var cell in _hexGrid.Cells)
        {
            if (cell != null && cell.IsWater)
            {
                waterCells.Add(cell);
            }
        }

        if (waterCells.Count == 0)
        {
            position = Vector3.zero;
            return false;
        }

        IHexCell randomCell = waterCells[Random.Range(0, waterCells.Count)];

        position = randomCell.CellTransform.position;
        position.y = _startingYPosition;

        return true;
    }
}
