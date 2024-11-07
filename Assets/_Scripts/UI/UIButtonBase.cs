using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIButtonBase : MonoBehaviour, IPointerEnterHandler
{
    private Button _button;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(SoundType.Hover);
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button?.onClick.AddListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
        AudioManager.Instance.PlaySFX(SoundType.Click);
        OnButtonClick();
    }

    protected abstract void OnButtonClick();
}
