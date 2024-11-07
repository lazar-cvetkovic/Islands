using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Main UI")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject[] _heartImages;

    [Header("Game Over UI")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _finalScoreText;
    [SerializeField] private TMP_Text _highScoreText;

    [Header("Settings UI")]
    [SerializeField] private GameObject _settingsPanel;

    [Header("Average Height UI")]
    [SerializeField] private GameObject _leftHeightObject;
    [SerializeField] private GameObject _rightHeightObject;
    [SerializeField] private TMP_Text _leftHeightText;
    [SerializeField] private TMP_Text _rightHeightText;

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScoreUI;
        GameManager.OnLivesChanged += UpdateLivesUI;
        GameManager.OnGameOver += ShowGameOverUI;
        GameManager.OnIslandAverageHeightShown += ShowCompareHeightUI;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScoreUI;
        GameManager.OnLivesChanged -= UpdateLivesUI;
        GameManager.OnGameOver -= ShowGameOverUI;
        GameManager.OnIslandAverageHeightShown -= ShowCompareHeightUI;
    }

    private void Start()
    {
        int initialScore = 0;
        int initialLives = _heartImages.Length;

        UpdateScoreUI(initialScore);
        UpdateLivesUI(initialLives, initialLives);

        _gameOverPanel.SetActive(false);
    }

    private void UpdateScoreUI(int score)
    {
        LeanTween.scale(_scoreText.gameObject, Vector3.one * 1.2f, 0.1f).setEaseOutExpo().setOnComplete(() =>
        {
            _scoreText.text = score.ToString();
            LeanTween.scale(_scoreText.gameObject, Vector3.one, 0.1f).setEaseOutExpo();
        });
    }

    private void UpdateLivesUI(int currentLives, int maxLives)
    {
        for (int i = 0; i < _heartImages.Length; i++)
        {
            if (i < currentLives)
            {
                _heartImages[i].SetActive(true);
            }
            else
            {
                _heartImages[i].SetActive(false);
            }
        }

        if (currentLives >= 0 && currentLives < _heartImages.Length)
        {
            LeanTween.scale(_heartImages[currentLives], Vector3.zero, 0.5f).setEaseInExpo();
        }
    }

    private void ShowGameOverUI(int finalScore, int highScore)
    {
        _finalScoreText.text = finalScore.ToString();
        _highScoreText.text = highScore.ToString();

        ShowPanel(_gameOverPanel);
    }

    private void ShowPanel(GameObject panel)
    {
        panel.transform.localScale = Vector3.zero;
        panel.SetActive(true);
        LeanTween.scale(panel, Vector3.one, 0.5f).setEaseOutExpo();
    }

    private void HidePanel(GameObject panel)
    {
        LeanTween.scale(panel, Vector3.zero, 0.5f).setEaseOutExpo()
                                                  .setOnComplete(() => panel.SetActive(false));
    }

    public void HandleRestartButtonClick()
    {
        _gameOverPanel.SetActive(false);
        foreach (var item in _heartImages)
        {
            item.SetActive(true);
            LeanTween.scale(item, Vector3.one, 0.5f).setEaseOutExpo();
        }


        GameManager.Instance.ResetGame();

        PlayerInputManager.Instance.SetInputEnabled(true);
    }

    public void HandleQuitButtonClick() => Application.Quit();

    private void ShowCompareHeightUI(float selectedIsland, float targetIsland)
    {
        SetAverageHeightText(selectedIsland, targetIsland);
        SlideAverageHeightUI();
    }

    private void SetAverageHeightText(float selectedIsland, float targetIsland)
    {
        float leftAverageHeight = selectedIsland;
        float rightAverageHeight = targetIsland;

        _leftHeightText.text = leftAverageHeight.ToString("F2");
        _rightHeightText.text = rightAverageHeight.ToString("F2");
    }

    private void SlideAverageHeightUI()
    {
        RectTransform leftRect = _leftHeightObject.GetComponent<RectTransform>();
        RectTransform rightRect = _rightHeightObject.GetComponent<RectTransform>();
        RectTransform parentRect = leftRect.parent.GetComponent<RectTransform>();

        float parentWidth = parentRect.rect.width;

        float leftStartX = -parentWidth / 2 - leftRect.rect.width;
        float rightStartX = parentWidth / 2 + rightRect.rect.width;

        leftRect.anchoredPosition = new Vector2(leftStartX, leftRect.anchoredPosition.y);
        rightRect.anchoredPosition = new Vector2(rightStartX, rightRect.anchoredPosition.y);

        float leftEndX = -parentWidth / 2 + leftRect.rect.width / 2;
        float rightEndX = parentWidth / 2 - rightRect.rect.width / 2;

        LeanTween.moveX(leftRect, leftEndX, 0.5f).setEaseOutExpo();
        LeanTween.moveX(rightRect, rightEndX, 0.5f).setEaseOutExpo();

        LeanTween.delayedCall(3f, () =>
        {
            LeanTween.moveX(leftRect, leftStartX, 0.5f).setEaseInExpo();
            LeanTween.moveX(rightRect, rightStartX, 0.5f).setEaseInExpo();
        });
    }

    public void HandleSettingsButtonClick()
    {
        PlayerInputManager.Instance.SetInputEnabled(false);
        ShowPanel(_settingsPanel);
    }

    public void HandleResumeButtonClick()
    {
        HidePanel(_settingsPanel);
        PlayerInputManager.Instance.SetInputEnabled(true);
    }

    public void ToggleSettings()
    {
        if (_settingsPanel.activeSelf)
        {
            HandleResumeButtonClick();
        }
        else
        {
            HandleSettingsButtonClick();
        }
    }
}
