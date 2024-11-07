using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject[] _heartImages; 

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _finalScoreText;
    [SerializeField] private TMP_Text _highScoreText;

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScoreUI;
        GameManager.OnLivesChanged += UpdateLivesUI;
        GameManager.OnGameOver += ShowGameOverUI;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScoreUI;
        GameManager.OnLivesChanged -= UpdateLivesUI;
        GameManager.OnGameOver -= ShowGameOverUI;
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
            LeanTween.scale(_heartImages[currentLives].gameObject, Vector3.zero, 0.2f).setEaseInExpo();
        }
    }

    private void ShowGameOverUI(int finalScore, int highScore)
    {
        _finalScoreText.text = finalScore.ToString();
        _highScoreText.text = highScore.ToString();

        _gameOverPanel.transform.localScale = Vector3.zero;
        _gameOverPanel.SetActive(true);
        LeanTween.scale(_gameOverPanel, Vector3.one, 0.5f).setEaseOutExpo();
    }

    public void HandleRestartButtonClick()
    {
        _gameOverPanel.SetActive(false);

        GameManager.Instance.ResetGame();

        PlayerInputManager.Instance.SetInputEnabled(true);
    }

    public void HandleQuitButtonClick() => Application.Quit();
}
