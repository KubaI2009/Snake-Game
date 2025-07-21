namespace SnakeGame.snake;

using SnakeGame.apple;
using SnakeGame.board;

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

    private static readonly TileType[] s_snakeTiles =
    {
        TileType.HeadLeft,
        TileType.HeadUp,
        TileType.HeadRight,
        TileType.HeadDown,
        TileType.Body
    };

    private static readonly int s_appleInterval = 7;
    private static readonly int s_specialAppleScore = 10;
    
    private Board _board;
    private Vector2Int _headPosition;
    
    private SnakeHead _head;
    private List<Vector2Int> _body;

    private bool _isAlive;

    private int _score;

    //private bool _ateApple;
    //private bool _ateDeliciousApple;
    //private bool _ateDisgustingApple;
    //private bool _ateRottenApple;
    //private bool _ateMagicApple;
    
    private Apple? _appleEaten;

    public Board Board
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
            
            _headPosition = new Vector2Int(x < _board.Width ? x >= 0 ? x : 0 : _board.Width, y < _board.Width ? y >= 0 ? y : 0 : _board.Width);
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
        get { return _score; }
        private set { _score = value; }
    }

    public Apple? AppleEaten
    {
        get { return _appleEaten; }
        private set { _appleEaten = value; }
    }
    
    public Snake(TileType[,] board, Vector2Int position, SnakeHead head) : this(SnakeGame.board.Board.Of(board), position, head) { }
    
    public Snake(Board board, Vector2Int position, SnakeHead head)
    {
        _board = board;
        _headPosition = position;
        _head = head;
        _body = new List<Vector2Int>() { _headPosition };
        _isAlive = true;
        _score = 0;
        _appleEaten = null;
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

        if (!IsAlive)
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
        
        if (AppleEaten == null)
        {
            newBody.RemoveAt(newBody.Count - 1);
        }
        
        //score increase
        if (!(AppleEaten is null))
        {
            Score += AppleEaten.Points();
        }
        
        if (AppleEaten is MagicApple)
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
        
        return new Vector2Int(random.Next(0, _board.Height), random.Next(0, _board.Height));
    }

    private void UpdateBoard()
    {
        //Console.WriteLine(ateApple);
        
        if (!_isAlive)
        {
            return;
        }
        
        _board.UpdateAndDeteriorate();
        
        for (int i = 0; i < Square(_board.Width); i++)
        {
            int y = i / _board.Width;
            int x = i % _board.Width;
            
            //Console.WriteLine($"{i} Y:{y} X:{x} {_board.GetLength(1)}");
            
            TileType tile = _board.GetTile(y, x);

            if (s_snakeTiles.Contains(tile))
            {
                _board.SetTile(y, x, TileType.Empty);
            }
        }

        //places the body
        foreach (Vector2Int piece in _body)
        {
            try
            {
                _board.SetTile(piece.Y, piece.X, TileType.Body);
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }

        //places the head
        try
        {
            _board.SetTile(_headPosition.Y, _headPosition.X, s_headTiles[_head]);
        }
        catch (IndexOutOfRangeException)
        {
            _isAlive = false;
        }
        
        //apples
        //RemoveTilesOfType(TileType.RottenApple, ticks % 100 == 0);
        //RemoveTilesOfType(TileType.MagicApple, ticks % 40 == 0);
        
        PlaceApple();
    }
    
    //generates an apple randomly
    //if _score is multiple of s_appleInterval it generates two
    private void PlaceApple()
    {
        Random random = new Random((int) DateTime.Now.Ticks);
        
        for (int i = 0; i <= (Score % s_appleInterval == 0 ? 1 : 0); i++)
        {
            if (!(AppleEaten is null) && !(AppleEaten is IKillingApple))
            {
                Vector2Int applePosition;

                while (true)
                {
                    applePosition = RandomPositionOnBoard();

                    if (!_body.Contains(applePosition) && _board.GetApple(applePosition) == null)
                    {
                        int j = new Random().Next(15);
                        
                        _board.AddApple(j == 0 ? new DeliciousApple(applePosition) : new Apple(applePosition));
                        //_board.SetTile(applePosition.Y, applePosition.X, appleTile);

                        if (!SpecialAppleCanAppear())
                        {
                            break;
                        }

                        Vector2Int specialApplePosition;

                        while (true)
                        {
                            specialApplePosition = RandomPositionOnBoard();

                            if (!_body.Contains(specialApplePosition)
                                && !applePosition.Equals(specialApplePosition)
                                && DoesNotShareAxisWithHead(specialApplePosition)
                                && _board.GetApple(specialApplePosition) == null)
                            {
                                break;
                            }
                        }

                        try
                        {
                            switch (random.Next(18))
                            {
                                case 0:
                                    _board.AddApple(new RottenApple(65, specialApplePosition));
                                    break;
                                case 1:
                                    _board.SetApple(new MagicApple(55, specialApplePosition));
                                    break;
                                case 2:
                                    _board.SetApple(new DisgustingApple(65 , specialApplePosition));
                                    break;
                            }
                        }
                        catch (AppleOverwritingException) { }
                        
                        break;
                    }
                }
            }
        }
    }

    private bool DoesNotShareAxisWithHead(Vector2Int position)
    {
        return position.X != _headPosition.X || position.Y != _headPosition.Y;
    }

    private bool SpecialAppleCanAppear()
    {
        return Score >= s_specialAppleScore;
    }

    private void RefluxApples()
    {
        //AteApple = false;
        //AteDeliciousApple = false;
        //AteDisgustingApple = false;
        //AteRottenApple = false;
        //AteMagicApple = false;

        AppleEaten = null;
    }
    
    public void Move()
    {
        RefluxApples();
        
        if (IsAlive)
        {
            UpdateBoard();
            
            Vector2Int newHeadPosition = GetNewHeadPosition();
            
            try
            {
                //AteApple = _board.GetTile(newHeadPosition.Y, newHeadPosition.X) == TileType.Apple;
                //AteDeliciousApple = _board.GetTile(newHeadPosition.Y, newHeadPosition.X) == TileType.DeliciousApple;
                //AteDisgustingApple = _board.GetTile(newHeadPosition.Y, newHeadPosition.X) == TileType.DisgustingApple;
                //AteRottenApple = _board.GetTile(newHeadPosition.Y, newHeadPosition.X) == TileType.RottenApple;
                //AteMagicApple = _board.GetTile(newHeadPosition.Y, newHeadPosition.X) == TileType.MagicApple;
                
                AppleEaten = _board.GetApple(newHeadPosition.Y, newHeadPosition.X);
                
                _isAlive = (_board.GetTile(newHeadPosition.Y, newHeadPosition.X) == TileType.Empty ||
                            (!(AppleEaten is IKillingApple) && !(AppleEaten is null)))
                           && IsInBoard();
                
                _board.RemoveAppleAt(newHeadPosition);
                //_board.SetTile(newHeadPosition, AteApple ? TileType.Empty : _board.GetTile(newHeadPosition);
            }
            catch (IndexOutOfRangeException)
            {
                RefluxApples();
            }
        
            UpdateBodyAndHead(newHeadPosition);
            
            UpdateBoard();
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
        
        for (int i = 0; i < _board.Height * _board.Width; i++)
        {
            //y - row
            //x - column

            int y = i / _board.Width;
            int x = i % _board.Width;

            if (_board.GetTile(y, x) == type)
            {
                _board.SetTile(y, x, TileType.Empty);
            }
        }
    }

    public Vector2Int GetNewHeadPosition()
    {
        Vector2Int newHeadPosition = Vector2Int.SumOfVectors(_headPosition, GoingDirection.Vector);
        
        newHeadPosition.Y = newHeadPosition.Y < 0 ? newHeadPosition.Y + _board.Height : newHeadPosition.Y;
        newHeadPosition.X = newHeadPosition.X < 0 ? newHeadPosition.X + _board.Width : newHeadPosition.X;
        
        newHeadPosition.Y %= _board.Height;
        newHeadPosition.X %= _board.Width;
        
        return newHeadPosition;
    }
    
    private bool IsInBoard()
    {
        return _headPosition.X < _board.Width
               && _headPosition.Y < _board.Height
               && _headPosition.X >= 0
               && _headPosition.Y >= 0;;
    }

    public void IncreaseScore(int i)
    {
        _score += i;
    }

    private void Die()
    {
        _isAlive = false;
    }

    private static int Square(int x)
    {
        return x * x;
    }

    private static int FloorDiv(int x, int y)
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