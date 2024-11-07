using System.Collections;
using UnityEngine;

public class WaterTile : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.05f;
    [SerializeField] private float frequency = 1f;

    private Vector3 _initialPosition;
    private Transform _transform;
    private bool _isWaveStarted;

    private void OnEnable()
    {
        StartCoroutine(InitializeWave());
    }
    private void Update()
    {
        HandleWaves();
    }

    private IEnumerator InitializeWave()
    {
        yield return new WaitForSeconds(1);

        _initialPosition = transform.position;
        _transform = transform;
        _isWaveStarted = true;
    }

    private void HandleWaves()
    {
        if (!_isWaveStarted) return;

        float newY = _initialPosition.y + amplitude * Mathf.Sin(frequency * Time.time);
        _transform.position = new Vector3(_initialPosition.x, newY, _initialPosition.z);
    }

}
