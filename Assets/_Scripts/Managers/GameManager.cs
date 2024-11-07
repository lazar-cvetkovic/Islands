using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DataLoader), typeof(HexGrid))]
public class GameManager : Singleton<GameManager>
{
    public static event Action<int> OnScoreChanged;
    public static event Action<int, int> OnLivesChanged; 
    public static event Action<int, int> OnGameOver;
    public static event Action<float, float> OnIslandAverageHeightShown;

    public const string HighScoreKey = "HighScore";
    public GameState CurrentState { get; private set; }

    [field: SerializeField]
    public bool IsOptimized { get; set; } = true;

    private int _lives = 3;
    private int _targetIslandId;

    private DataLoader _dataLoader;
    private HexGrid _hexGrid;
    private IslandDetector _islandDetector;

    private int _score = 0;
    private WaitForSeconds _islandSelectionWaitTime = new(3);

    protected override void Awake()
    {
        base.Awake();

        _dataLoader = GetComponent<DataLoader>();
        _hexGrid = GetComponent<HexGrid>();
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        CurrentState = GameState.Loading;

        _hexGrid.ClearGrid();
        _dataLoader.LoadData(HandleLoadedData);
    }

    private void HandleLoadedData(bool isDataLoadedCorrectly)
    {
        if (!isDataLoadedCorrectly)
        {
            // TODO: Add bad data UI feedback
            return;
        }

        _hexGrid.GenerateGrid(_dataLoader.HeightMap);

        _islandDetector = new IslandDetector(_hexGrid.Cells, _hexGrid.Width, _hexGrid.Height);
        _islandDetector.DetectIslands();
        _islandDetector.CalculateAverageHeights();

        _targetIslandId = _islandDetector.GetHighestAverageIslandId();

        CurrentState = GameState.Playing;
        PlayerInputManager.Instance.SetInputEnabled(true);

        OnLivesChanged?.Invoke(_lives, 3);
    }

    public void HandleCellClick(IHexCell cell)
    {
        AudioManager.Instance.PlaySFX(SoundType.Click);

        if (CurrentState != GameState.Playing || cell.IsWater) return;

        int selectedIslandId = cell.IslandId;

        CurrentState = GameState.Paused;

        PlayerInputManager.Instance.SetInputEnabled(false);

        StartCoroutine(HandleIslandSelection(selectedIslandId));
    }

    private IEnumerator HandleIslandSelection(int selectedIslandId)
    {
        SetIslandColor(selectedIslandId);
        SendUIFeedbackForSelectedIsland(selectedIslandId);

        yield return _islandSelectionWaitTime;

        if (selectedIslandId == _targetIslandId)
        {
            _score++;
            OnScoreChanged?.Invoke(_score);

            InitializeGame();

            CurrentState = GameState.Playing;
            AudioManager.Instance.PlaySFX(SoundType.Win);
        }
        else
        {
            _lives--;
            OnLivesChanged?.Invoke(_lives, 3);

            if (_lives <= 0)
            {
                int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
                if (_score > highScore)
                {
                    PlayerPrefs.SetInt(HighScoreKey, _score);
                    highScore = _score;
                }

                OnGameOver?.Invoke(_score, highScore);

                CurrentState = GameState.GameOver;
                AudioManager.Instance.PlaySFX(SoundType.LooseFinal);
            }
            else
            {
                InitializeGame();

                CurrentState = GameState.Playing;

                PlayerInputManager.Instance.SetInputEnabled(true);
                AudioManager.Instance.PlaySFX(SoundType.LooseHeart);
            }
        }
    }

    private void SendUIFeedbackForSelectedIsland(int selectedIslandId)
    {
        float currentIslandHeight = _islandDetector.GetAverageHeight(selectedIslandId);
        float targetIslandHeight = _islandDetector.GetAverageHeight(_targetIslandId);

        OnIslandAverageHeightShown?.Invoke(currentIslandHeight, targetIslandHeight);
    }

    private void SetIslandColor(int selectedIslandId)
    {
        ChangeIslandColor(_targetIslandId, Color.green);

        if (selectedIslandId != _targetIslandId)
        {
            ChangeIslandColor(selectedIslandId, Color.red);
        }
    }

    private void ChangeIslandColor(int islandId, Color color)
    {
        var islandCells = GetIslandCells(islandId);
        foreach (var cell in islandCells)
        {
            cell.SetHighlightColor(color);
        }
    }

    public List<IHexCell> GetIslandCells(int islandId)
    {
        if (_islandDetector.Islands.TryGetValue(islandId, out var islandCells))
        {
            return islandCells;
        }
        return new List<IHexCell>();
    }

    public void ResetGame()
    {
        _score = 0;
        _lives = 3;

        InitializeGame();

        OnScoreChanged?.Invoke(_score);
        OnLivesChanged?.Invoke(_lives, 3);
    }
}

public enum GameState
{
    Loading = 0,
    Playing = 1,
    Paused = 2,
    GameOver = 3
}
