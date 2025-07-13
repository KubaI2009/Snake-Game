using System.Numerics;

namespace SnakeGame;

public class Snake
{
    //I know this could probably be like two dictionaries at most
    //I don't care though, it works
    //don't touch it
    private static readonly Dictionary<TileType, SnakeHead> s_heads = new Dictionary<TileType, SnakeHead>()
    {
        { TileType.HeadLeft , SnakeHead.Left},
        { TileType.HeadUp , SnakeHead.Up},
        { TileType.HeadRight , SnakeHead.Right},
        { TileType.HeadDown , SnakeHead.Down}
    };

    private static readonly Dictionary<SnakeHead, CardinalDirection> s_directions = new Dictionary<SnakeHead, CardinalDirection>()
    {
        { SnakeHead.Left , CardinalDirection.Left},
        { SnakeHead.Up , CardinalDirection.Up},
        { SnakeHead.Right , CardinalDirection.Right},
        { SnakeHead.Down , CardinalDirection.Down}
    };
    
    private static readonly Dictionary<CardinalDirection, SnakeHead> s_headsFromDirections = new Dictionary<CardinalDirection, SnakeHead>()
    {
        {CardinalDirection.Left, SnakeHead.Left},
        {CardinalDirection.Up, SnakeHead.Up},
        {CardinalDirection.Right, SnakeHead.Right},
        {CardinalDirection.Down, SnakeHead.Down}
    };

    private static readonly Dictionary<SnakeHead, TileType> s_headTiles = new Dictionary<SnakeHead, TileType>()
    {
        { SnakeHead.Left , TileType.HeadLeft},
        { SnakeHead.Up , TileType.HeadUp},
        { SnakeHead.Right , TileType.HeadRight},
        { SnakeHead.Down , TileType.HeadDown}
    };

    private static readonly int s_appleInterval = 7;
    
    private TileType[,] _board;
    private Vector2Int _headPosition;
    
    private SnakeHead _head;
    private List<Vector2Int> _body;

    private bool _isAlive;

    public TileType[,] Board
    {
        get { return _board; }
    }

    public Vector2Int HeadPosition
    {
        get { return _headPosition; }
        set
        {
            int x = value.X;
            int y = value.Y;
            
            _headPosition = new Vector2Int(x < _board.GetLength(1) ? x >= 0 ? x : 0 : _board.GetLength(1), y < _board.GetLength(1) ? y >= 0 ? y : 0 : _board.GetLength(1));
        }
    }

    public SnakeHead Head
    {
        get { return _head; }
        set
        {
            _head = value;
            UpdateBodyAndHead(_headPosition, true);
        }
    }

    public List<Vector2Int> Body
    {
        get { return _body; }
    }

    public bool IsAlive
    {
        get { return _isAlive; }
    }

    public CardinalDirection GoingDirection
    {
        get { return s_directions[_head]; }
        set { _head = s_headsFromDirections[value]; }
    }

    public int Score
    {
        get { return _body.Count - 1; }
    }
    
    public Snake(TileType[,] board, Vector2Int position, SnakeHead head)
    {
        _board = board;
        _headPosition = position;
        _head = head;
        _body = new List<Vector2Int>() { _headPosition };
        _isAlive = true;
    }
    
    //so basically the snake gets some head
    //I know this sounds bad without context
    private static SnakeHead GetHead(TileType headTile)
    {
        try
        {
            return s_heads[headTile];
        }
        catch (ArgumentOutOfRangeException)
        {
            return SnakeHead.Right;
        }
    }
    
    /*public void SetGoingDirection(CardinalDirection direction)
    {
        _head = s_headsFromDirections[direction];
    }

    public CardinalDirection GetGoingDirection()
    {
        return s_directions[_head];
    }*/

    private void UpdateBodyAndHead(Vector2Int newHeadPosition, bool ateApple)
    {
        //index 0 is always the head

        if (!_isAlive)
        {
            return;
        }
        
        //moves the body "forward"
        List<Vector2Int> newBody = new List<Vector2Int>()
        {
            newHeadPosition
        };

        foreach (Vector2Int segment in _body)
        {
            newBody.Add(segment);
        }

        if (!ateApple)
        {
            newBody.RemoveAt(newBody.Count - 1);
        }
        
        //Print(newBody);
        
        _headPosition = newHeadPosition;
        _body =  newBody;
    }

    private void Print(List<Vector2Int> body)
    {
        Console.Write("[ ");
        
        foreach (Vector2Int piece in body)
        {
            Console.Write($"({piece.X}, {piece.Y}) ");
        }
        
        Console.WriteLine("]");
    }

    private Vector2Int RandomPosition()
    {
        Random random = new Random((int) DateTime.Now.Ticks);
        
        return new Vector2Int(random.Next(0, _board.GetLength(0)), random.Next(0, _board.GetLength(0)));
    }

    private void UpdateBoard(bool ateApple)
    {
        //Console.WriteLine(ateApple);
        
        if (!_isAlive)
        {
            return;
        }
        
        for (int i = 0; i < Square(_board.GetLength(1)); i++)
        {
            int y = i / _board.GetLength(1);
            int x = i % _board.GetLength(1);
            
            //Console.WriteLine($"{i} Y:{y} X:{x} {_board.GetLength(1)}");
            
            TileType tile = _board[y, x];

            if (tile != TileType.Empty && tile != TileType.Apple)
            {
                _board[y, x] = TileType.Empty;
            }
        }

        //places the body
        foreach (Vector2Int piece in _body)
        {
            try
            {
                _board[piece.Y, piece.X] = TileType.Body;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        //places the head
        try
        {
            _board[_headPosition.Y, _headPosition.X] = s_headTiles[_head];
        }
        catch (IndexOutOfRangeException)
        {
            _isAlive = false;
        }
        
        //generates an apple randomly
        //if _score is multiple of s_appleInterval it generates two
        for (int i = 0; i <= (Score % s_appleInterval == 0 ? 1 : 0); i++)
        {
            if (ateApple)
            {
                Vector2Int applePosition;

                while (true)
                {
                    applePosition = RandomPosition();

                    if (!_body.Contains(applePosition))
                    {
                        _board[applePosition.Y, applePosition.X] = TileType.Apple;
                        break;
                    }
                }
            }
        }
    }

    public void Move()
    {
        if (_isAlive)
        {
            Vector2Int newHeadPosition = Vector2Int.SumOfVectors(_headPosition, GoingDirection.Vector);
            bool ateApple;
            
            try
            {
                ateApple = _board[newHeadPosition.Y, newHeadPosition.X] == TileType.Apple;
                _isAlive = (_board[newHeadPosition.Y, newHeadPosition.X] == TileType.Empty || ateApple) && IsInBoard();
            }
            catch (IndexOutOfRangeException)
            {
                ateApple = false;
            }
        
            UpdateBodyAndHead(newHeadPosition, ateApple);
            
            UpdateBoard(ateApple);
        }
    }

    private bool IsInBoard()
    {
        return _headPosition.X < _board.GetLength(1)
               && _headPosition.Y < _board.GetLength(0)
               && _headPosition.X >= 0
               && _headPosition.Y >= 0;;
    }

    private void Die()
    {
        _isAlive = false;
    }

    private int Square(int x)
    {
        return x * x;
    }

    private int FloorDiv(int x, int y)
    {
        int result = 0;

        while (x > 0)
        {
            x -= y;
            
            result++;
        }
        
        return result;
    }
}