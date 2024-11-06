using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private HexCell _currentHoveredCell;
    private int _currentHoveredIslandId = -1;
    private int _selectedIslandId = -1;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.ToggleCamera.performed += ToggleCamera;
        _inputActions.Player.Click.performed += HandleClick;
        _inputActions.Player.MouseMove.performed += HandleMouseHover;
    }

    private void OnDisable()
    {
        _inputActions.Player.ToggleCamera.performed -= ToggleCamera;
        _inputActions.Player.Click.performed -= HandleClick;
        _inputActions.Player.MouseMove.performed -= HandleMouseHover;
        _inputActions.Player.Disable();
    }

    private void ToggleCamera(InputAction.CallbackContext context)
    {
        CameraCoordinator.Instance.ToggleCamera();
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = _inputActions.Player.MousePosition.ReadValue<Vector2>();
        Camera mainCamera = CameraCoordinator.Instance.MainCamera;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log(hit.collider.gameObject.name);
            HexCell hexCell = hit.collider.GetComponentInParent<HexCell>();
            if (hexCell != null)
            {
                GameManager.Instance.HandleCellClick(hexCell);
            }
        }
    }

    private void HandleMouseHover(InputAction.CallbackContext context)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = CameraCoordinator.Instance.MainCamera.ScreenPointToRay(mousePosition);

        int layerMask = LayerMask.GetMask("HexCellLayer");
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            HexCell hexCell = hit.collider.GetComponentInParent<HexCell>();
            if (hexCell != null)
            {
                if (_currentHoveredCell != hexCell)
                {
                    int newIslandId = hexCell.IslandId;

                    if (_currentHoveredIslandId != newIslandId && newIslandId != _selectedIslandId)
                    {
                        UnhighlightCurrentIsland();

                        _currentHoveredCell = hexCell;
                        _currentHoveredIslandId = newIslandId;

                        HighlightIsland(_currentHoveredIslandId);
                    }
                }
            }
            else
            {
                UnhighlightCurrentIsland();
            }
        }
        else
        {
            UnhighlightCurrentIsland();
        }
    }

    private void HighlightIsland(int islandId)
    {
        if (islandId == -1)
            return;

        var islandCells = GameManager.Instance.GetIslandCells(islandId);
        foreach (var cell in islandCells)
        {
            cell.Highlight();
        }
    }

    private void UnhighlightCurrentIsland()
    {
        if (_currentHoveredIslandId != -1 && _currentHoveredIslandId != _selectedIslandId)
        {
            var islandCells = GameManager.Instance.GetIslandCells(_currentHoveredIslandId);
            foreach (var cell in islandCells)
            {
                cell.Unhighlight();
            }
        }

        _currentHoveredCell = null;
        _currentHoveredIslandId = -1;
    }
}
