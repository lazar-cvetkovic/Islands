using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : UIButtonBase
{
    private Vector3 _startingScale;

    protected override void OnButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        _startingScale = gameObject.transform.localScale;
        AnimateButton();
    }

    private void AnimateButton()
    {
        LeanTween.scale(gameObject, 0.9f * _startingScale, 0.5f)
                 .setEaseInOutSine()
                 .setLoopPingPong();
    }

}
