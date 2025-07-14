namespace SnakeGame.snake;

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
    private static readonly int s_specialAppleScore = 10;
    
    private TileType[,] _board;
    private Vector2Int _headPosition;
    
    private SnakeHead _head;
    private List<Vector2Int> _body;

    private bool _isAlive;

    private bool _ateApple;
    private bool _ateRottenApple;
    private bool _ateMagicApple;

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
            UpdateBodyAndHead(_headPosition);
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

    public bool AteApple
    {
        get { return _ateApple; }
        set { _ateApple = value; }
    }

    public bool AteRottenApple
    {
        get { return _ateRottenApple; }
        set { _ateRottenApple = value; }
    }

    public bool AteMagicApple
    {
        get { return _ateMagicApple; }
        set
        {
            _ateMagicApple = value;
            AteApple = _ateMagicApple || AteApple;
        }
    }
    
    public Snake(TileType[,] board, Vector2Int position, SnakeHead head)
    {
        _board = board;
        _headPosition = position;
        _head = head;
        _body = new List<Vector2Int>() { _headPosition };
        _isAlive = true;
        _ateApple = false;
        _ateRottenApple = false;
        _ateMagicApple = false;
    }
    
    //so basically the snake gets some head
    //I know this sounds bad without context
    private static SnakeHead HeadOf(TileType headTile)
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

    private void UpdateBodyAndHead(Vector2Int newHeadPosition)
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

        if (!_ateApple)
        {
            newBody.RemoveAt(newBody.Count - 1);
        }

        if (_ateMagicApple)
        {
            for (int i = 1; i < 5; i++)
            {
                if (newBody.Count - i > 0)
                {
                    try
                    {
                        newBody.RemoveAt(newBody.Count - i);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }
                }
            }
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

    public Vector2Int RandomPositionOnBoard()
    {
        return RandomPositionOnBoard(0);
    }

    public Vector2Int RandomPositionOnBoard(int i)
    {
        Random random = new Random((int) DateTime.Now.Ticks + i);
        
        return new Vector2Int(random.Next(0, _board.GetLength(0)), random.Next(0, _board.GetLength(0)));
    }

    private void UpdateBoard(int ticks)
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

            if (tile != TileType.Empty
                && tile != TileType.Apple
                && tile != TileType.RottenApple
                && tile != TileType.MagicApple)
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
        
        //apples
        RemoveTilesOfType(TileType.RottenApple, ticks % 100 == 0);
        RemoveTilesOfType(TileType.MagicApple, ticks % 40 == 0);
        
        PlaceApple();
    }
    
    //generates an apple randomly
    //if _score is multiple of s_appleInterval it generates two
    private void PlaceApple()
    {
        Random random = new Random((int) DateTime.Now.Ticks);
        TileType apple = TileType.Apple;

        if (random.Next(10) == 0 && SpecialAppleCanAppear())
        {
            apple = TileType.MagicApple;
        }
        
        for (int i = 0; i <= (Score % s_appleInterval == 0 ? 1 : 0); i++)
        {
            if (AteApple)
            {
                Vector2Int applePosition;

                while (true)
                {
                    applePosition = RandomPositionOnBoard();

                    if (!_body.Contains(applePosition))
                    {
                        _board[applePosition.Y, applePosition.X] = apple;

                        if (random.Next(10) == 0 && SpecialAppleCanAppear())
                        {
                            Vector2Int rottenApplePosition;

                            while (true)
                            {
                                rottenApplePosition = RandomPositionOnBoard();

                                if (!_body.Contains(rottenApplePosition) && !applePosition.Equals(rottenApplePosition))
                                {
                                    _board[rottenApplePosition.Y, rottenApplePosition.X] = TileType.Body;
                                    
                                    break;
                                }
                            }
                        }
                        
                        break;
                    }
                }
            }
        }
    }

    private bool SpecialAppleCanAppear()
    {
        return Score >= s_specialAppleScore;
    }

    private void RefluxApples()
    {
        AteApple = false;
        AteRottenApple = false;
        AteMagicApple = false;
    }
    
    public void Move(int ticks)
    {
        if (_isAlive)
        {
            Vector2Int newHeadPosition = GetNewHeadPosition();
            bool ateApple;
            bool ateRottenApple;
            bool ateMagicApple = false;
            
            try
            {
                AteApple = _board[newHeadPosition.Y, newHeadPosition.X] == TileType.Apple;
                AteRottenApple = _board[newHeadPosition.Y, newHeadPosition.X] == TileType.RottenApple;
                AteMagicApple = _board[newHeadPosition.Y, newHeadPosition.X] == TileType.MagicApple;
                
                _isAlive = (_board[newHeadPosition.Y, newHeadPosition.X] == TileType.Empty || AteApple) && IsInBoard() && !AteRottenApple;
            }
            catch (IndexOutOfRangeException)
            {
                _ateApple = false;
            }
        
            UpdateBodyAndHead(newHeadPosition);
            
            UpdateBoard(ticks);
            
            RefluxApples();
        }
    }

    public void Kill()
    {
        _isAlive = false;
    }

    private void RemoveTilesOfType(TileType type, bool remove)
    {
        if (!remove)
        {
            return;
        }
        
        for (int i = 0; i < _board.GetLength(0) * _board.GetLength(1); i++)
        {
            //y - row
            //x - column

            int y = i / _board.GetLength(0);
            int x = i % _board.GetLength(1);

            if (_board[y, x] == type)
            {
                _board[y, x] = TileType.Empty;
            }
        }
    }

    public Vector2Int GetNewHeadPosition()
    {
        Vector2Int newHeadPosition = Vector2Int.SumOfVectors(_headPosition, GoingDirection.Vector);
        
        newHeadPosition.Y = newHeadPosition.Y < 0 ? newHeadPosition.Y + _board.GetLength(0) : newHeadPosition.Y;
        newHeadPosition.X = newHeadPosition.X < 0 ? newHeadPosition.X + _board.GetLength(1) : newHeadPosition.X;
        
        newHeadPosition.Y %= _board.GetLength(0);
        newHeadPosition.X %= _board.GetLength(1);
        
        return newHeadPosition;
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