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

    public const string HighScoreKey = "HighScore";
    public GameState CurrentState { get; private set; }

    private int _lives = 3;
    private int _targetIslandId;

    private DataLoader _dataLoader;
    private HexGrid _hexGrid;
    private IslandDetector _islandDetector;

    private int _score = 0;
    private WaitForSeconds _islandSelectionWaitTime = new(2);


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

    public void HandleCellClick(HexCell cell)
    {
        if (CurrentState != GameState.Playing || cell.IsWater) return;

        int selectedIslandId = cell.IslandId;

        CurrentState = GameState.Paused;

        PlayerInputManager.Instance.SetInputEnabled(false);

        StartCoroutine(HandleIslandSelection(selectedIslandId));
    }

    private IEnumerator HandleIslandSelection(int selectedIslandId)
    {
        HighlightIsland(_targetIslandId, Color.green);

        if (selectedIslandId != _targetIslandId)
        {
            HighlightIsland(selectedIslandId, Color.red);
        }

        yield return _islandSelectionWaitTime;

        if (selectedIslandId == _targetIslandId)
        {
            _score++;
            OnScoreChanged?.Invoke(_score);

            InitializeGame();

            CurrentState = GameState.Playing;
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
            }
            else
            {
                InitializeGame();

                CurrentState = GameState.Playing;

                PlayerInputManager.Instance.SetInputEnabled(true);
            }
        }
    }

    private void HighlightIsland(int islandId, Color color)
    {
        var islandCells = GetIslandCells(islandId);
        foreach (var cell in islandCells)
        {
            cell.SetHighlightColor(color);
        }
    }

    public List<HexCell> GetIslandCells(int islandId)
    {
        if (_islandDetector.Islands.TryGetValue(islandId, out var islandCells))
        {
            return islandCells;
        }
        return new List<HexCell>();
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
