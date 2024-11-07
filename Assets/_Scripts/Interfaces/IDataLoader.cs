using System;

public interface IDataLoader
{
    int[,] HeightMap { get; }

    void LoadData(Action<bool> onComplete);
}
