using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CameraButton : MonoBehaviour
{
    [SerializeField] Image _cameraImage;
    [SerializeField] Sprite _camera1Sprite;
    [SerializeField] Sprite _camera2Sprite;

    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleButtonClick);
    }

    private void OnEnable()
    {
        CameraCoordinator.CameraStateChanged += HandleCameraChanged;
    }

    private void OnDisable()
    {
        CameraCoordinator.CameraStateChanged -= HandleCameraChanged;
    }

    private void HandleCameraChanged(bool cameraValue) => _cameraImage.sprite = cameraValue ? _camera1Sprite : _camera2Sprite;

    private void HandleButtonClick() => CameraCoordinator.Instance.ToggleCamera();
}
