using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DataLoader), typeof(HexGrid))]
public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }

    private int _guessesLeft = 3;
    private int _targetIslandId;

    private DataLoader _dataLoader;
    private HexGrid _hexGrid;
    private IslandDetector _islandDetector;

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
        _dataLoader.LoadData(HandleLoadedData);
    }

    private void EndGame(bool isWin)
    {
        CurrentState = GameState.GameOver;
        if (isWin)
        {
            // TODO: Add win logic
            Debug.Log("You found the highest island!");
        }
        else
        {
            // TODO: Add loose logic
            Debug.Log("Game Over!");
        }
    }

    private void HandleLoadedData(bool isDataLoadedCorrectly)
    {
        if(!isDataLoadedCorrectly)
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
    }

    public void HandleCellClick(HexCell cell)
    {
        if (CurrentState != GameState.Playing || cell.IsWater) return;

        int selectedIslandId = cell.IslandId;
        if (selectedIslandId == _targetIslandId)
        {
            EndGame(true);
        }
        else
        {
            _guessesLeft--;
            if (_guessesLeft <= 0)
            {
                EndGame(false);
            }
            else
            {
                // TODO: Provide feedback to the player
            }
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
}

public enum GameState 
{
    Loading = 0, 
    Playing = 1, 
    GameOver = 2
}

