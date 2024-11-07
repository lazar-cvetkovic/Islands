using UnityEngine;

public class QuitButton : UIButtonBase
{
    protected override void OnButtonClick()
    {
        UIManager.Instance.HandleQuitButtonClick();
    }
}
