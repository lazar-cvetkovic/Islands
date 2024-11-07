using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptimizationButton : UIButtonBase
{
    [SerializeField] private Sprite _optimizedSprite;
    [SerializeField] private Sprite _nonOptimizedSprite;

    private Image _image;
    private TMP_Text _buttonText;
    private bool _isOptimizedMode;

    const string OptimizedText = "TURN OFF\r\nOPTIMIZATION MODE";
    const string NonOptimizedText = "TURN ON\r\nOPTIMIZATION MODE";

    private void Start()
    {
        _image = GetComponent<Image>();
        _buttonText = GetComponentInChildren<TMP_Text>();

        _isOptimizedMode = GameManager.Instance.IsOptimized;
        UpdateOptimizationState();
    }

    protected override void OnButtonClick()
    {
        ToggleOptimizationState();
    }

    private void ToggleOptimizationState()
    {
        _isOptimizedMode = !_isOptimizedMode;
        UpdateOptimizationState();
    }

    private void UpdateOptimizationState()
    {
        _image.sprite = _isOptimizedMode ? _optimizedSprite : _nonOptimizedSprite;
        _buttonText.text = _isOptimizedMode ? OptimizedText : NonOptimizedText;

        GameManager.Instance.IsOptimized = _isOptimizedMode;
    }
}
