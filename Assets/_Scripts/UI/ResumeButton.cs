using UnityEngine;

public class ResumeButton : UIButtonBase
{
    protected override void OnButtonClick()
    {
        UIManager.Instance.HandleResumeButtonClick();
    }
}
