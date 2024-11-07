using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CameraButton : UIButtonBase
{
    [SerializeField] private Image _cameraImage;
    [SerializeField] private Sprite _camera1Sprite;
    [SerializeField] private Sprite _camera2Sprite;

    private void OnEnable()
    {
        CameraCoordinator.CameraStateChanged += HandleCameraChanged;
    }

    private void OnDisable()
    {
        CameraCoordinator.CameraStateChanged -= HandleCameraChanged;
    }

    private void HandleCameraChanged(bool cameraValue) => _cameraImage.sprite = cameraValue ? _camera1Sprite : _camera2Sprite;

    protected override void OnButtonClick()
    {
        CameraCoordinator.Instance.ToggleCamera();
    }
}
