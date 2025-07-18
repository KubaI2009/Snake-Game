﻿using SnakeGame.apple;

namespace SnakeGame.board;

public class Board
{
    private static readonly TileType[] s_appleRepleacable =
    {
        TileType.Empty,
        TileType.RottenApple
    };
    
    private TileType[,] _board;
    private Apple?[,] _appleMatrix;

    public Apple[] Apples
    {
        get
        {
            List<Apple> apples = new List<Apple>();

            foreach (Apple? apple in _appleMatrix)
            {
                if (apple != null)
                {
                    apples.Add(apple);
                }
            }
            
            return apples.ToArray();
        }
    }

    public int Height
    {
        get { return _board.GetLength(0); }
    }

    public int Width
    {
        get { return _board.GetLength(1); }
    }
    
    //y - height
    //x - width
    public Board(int height, int width)
    {
        _board = new TileType[height, width];
        _appleMatrix = new Apple?[height, width];

        for (int i = 0; i < height * width; i++)
        {
            int y = i / width;
            int x = i % width;

            //Console.WriteLine($"({y}, {x})");
            
            _board[y, x] = TileType.Empty;
            _appleMatrix[y, x] = null;
        }
    }

    public void AddApple(Apple apple)
    {
        if (_appleMatrix[apple.Y, apple.X] != null)
        {
            throw new AppleOverwritingException();
        }
        
        SetApple(apple);
    }

    public bool RemoveAppleAt(Vector2Int position)
    {
        return RemoveAppleAt(position.Y, position.X);
    }

    public bool RemoveAppleAt(int y, int x)
    {
        if (_appleMatrix[y, x] != null)
        {
            //Console.WriteLine("test");
            
            _appleMatrix[y, x] = null;
            
            Update();
            
            return true;
        }
        
        return false;
    }

    public void SetApple(Apple apple)
    {
        _appleMatrix[apple.Y, apple.X] = apple;
        
        Update();
    }

    public Apple? GetApple(int y, int x)
    {
        return _appleMatrix[y, x];
    }

    public Apple? GetApple(Vector2Int position)
    {
        return GetApple(position.Y, position.X);
    }

    public void SetTile(int y, int x, TileType type)
    {
        _board[y, x] = type;
    }

    public void SetTile(Vector2Int position, TileType type)
    {
        SetTile(position.Y, position.X, type);
    }

    public TileType GetTile(int y, int x)
    {
        return _board[y, x];
    }

    public TileType GetTile(Vector2Int position)
    {
        return GetTile(position.Y, position.X);
    }

    public void UpdateAndDeteriorate()
    {
        DeteriorateApples();
        Update();
    }

    public void Update()
    {
        foreach (Apple apple in Apples)
        {
            if (apple is SpecialApple specialApple && !specialApple.IsAlive)
            {
                _appleMatrix[specialApple.Y, specialApple.X] = null;
                
                SetTile(specialApple.Y, specialApple.X, TileType.Empty);
            }
        }
        
        for (int i = 0; i < Height * Width; i++)
        {
            int y = i / Width;
            int x = i % Width;
            
            SetTile(y, x, GetApple(y, x) != null && s_appleRepleacable.Contains(GetTile(y, x)) ? GetApple(y, x).ToTileType() : GetTile(y, x));
        }
    }

    public void DeteriorateApples()
    {
        foreach (Apple apple in Apples)
        {
            if (apple is SpecialApple specialApple)
            {
                specialApple.DecreaseLifeTime(1);

                //Console.WriteLine($"{specialApple.GetType()}: {specialApple.LifeTime}");
            }
        }
    }

    public static Board Of(TileType[,] board)
    {
        return new Board(board.GetLength(0), board.GetLength(1));
    }
}