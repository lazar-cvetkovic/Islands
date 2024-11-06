using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool isTopDownCamera = true;
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 40f;
    [SerializeField] private float _panSpeed = 4f;

    private bool _isPanning = false;
    private PlayerInputActions _inputActions;
    private Transform _transform;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _transform = transform;
    }

    private void OnEnable()
    {
        _inputActions.Camera.Enable();

        _inputActions.Camera.Zoom.performed += OnZoom;
        _inputActions.Camera.PanStart.performed += OnPanStart;
        _inputActions.Camera.PanEnd.performed += OnPanEnd;
        _inputActions.Camera.Pan.performed += OnPan;
    }

    private void OnDisable()
    {
        _inputActions.Camera.Zoom.performed -= OnZoom;
        _inputActions.Camera.PanStart.performed -= OnPanStart;
        _inputActions.Camera.PanEnd.performed -= OnPanEnd;
        _inputActions.Camera.Pan.performed -= OnPan;

        _inputActions.Camera.Disable();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float scrollData = context.ReadValue<float>();

        if (Mathf.Abs(scrollData) > 0.01f)
        {
            if (isTopDownCamera)
            {
                // Zoom handled on Y axis
                float currentY = _transform.localPosition.y;
                float targetY = currentY + -scrollData * _zoomSpeed;
                targetY = Mathf.Clamp(targetY, _minZoom, _maxZoom);

                LeanTween.cancel(_transform.gameObject);
                LeanTween.moveLocalY(_transform.gameObject, targetY, 0.2f);
            }
            else
            {
                // Zoom handled on Z axis
                float currentZ = _transform.localPosition.z;
                float targetZ = currentZ + scrollData * _zoomSpeed;
                targetZ = Mathf.Clamp(targetZ, _minZoom, _maxZoom);

                LeanTween.cancel(_transform.gameObject);
                LeanTween.moveLocalZ(_transform.gameObject, targetZ, 0.2f);
            }
        }
    }

    private void OnPanStart(InputAction.CallbackContext context)
    {
        _isPanning = true;
    }

    private void OnPanEnd(InputAction.CallbackContext context)
    {
        _isPanning = false;
    }

    private void OnPan(InputAction.CallbackContext context)
    {
        if (_isPanning)
        {
            Vector2 mouseDelta = context.ReadValue<Vector2>();

            if (isTopDownCamera)
            {
                var move = new Vector3(-mouseDelta.x * _panSpeed * Time.deltaTime, 0, -mouseDelta.y * _panSpeed * Time.deltaTime);
                _transform.Translate(move, Space.World);
            }
            else
            {
                Vector3 right = _transform.right;
                Vector3 up = _transform.up;
                Vector3 move = (-mouseDelta.x * right + -mouseDelta.y * up) * _panSpeed * Time.deltaTime;
                _transform.Translate(move, Space.World);
            }
        }
    }
}