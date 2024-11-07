using UnityEngine;

public class SettingsButton : UIButtonBase
{
    protected override void OnButtonClick()
    {
        UIManager.Instance.HandleSettingsButtonClick();
    }
}
