using System;
using UnityEngine;

public class CameraCoordinator : Singleton<CameraCoordinator>
{
    public static event Action<bool> CameraStateChanged;

    [SerializeField] private Camera _topDownCamera;
    [SerializeField] private Camera _isometricCamera;

    private bool _isTopDownCamera;

    public Camera MainCamera => _isTopDownCamera ? _topDownCamera : _isometricCamera;

    private void Start()
    {
        SetCameraState(isTopDownCamera: true);
    }

    private void SetCameraState(bool isTopDownCamera)
    {
        _isTopDownCamera = isTopDownCamera;
        _topDownCamera.gameObject.SetActive(isTopDownCamera);
        _isometricCamera.gameObject.SetActive(!isTopDownCamera);

        CameraStateChanged?.Invoke(_isTopDownCamera);
    }

    public void ToggleCamera() => SetCameraState(!_isTopDownCamera);
}
