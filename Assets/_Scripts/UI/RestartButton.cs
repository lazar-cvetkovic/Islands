using UnityEngine;

public class RestartButton : UIButtonBase
{
    protected override void OnButtonClick()
    {
        UIManager.Instance.HandleRestartButtonClick();
    }
}
