using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SnakeGame.snake;
using SnakeGame.board;
using SnakeGame.apple;
using Vector = System.Numerics.Vector;

namespace SnakeGame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class Root : Window
{
    private static readonly Dictionary<Key, CardinalDirection> s_directions = new Dictionary<Key, CardinalDirection>()
    {
        {Key.Left, CardinalDirection.Left},
        {Key.Up, CardinalDirection.Up},
        {Key.Right, CardinalDirection.Right},
        {Key.Down, CardinalDirection.Down},
        {Key.A, CardinalDirection.Left},
        {Key.W, CardinalDirection.Up},
        {Key.D, CardinalDirection.Right},
        {Key.S, CardinalDirection.Down}
    };
    
    private static readonly Dictionary<TileType, TileStyleData> s_styles = new Dictionary<TileType, TileStyleData>()
    {
        { TileType.Empty , new TileStyleData(new SolidColorBrush(Colors.Black), "")},
        { TileType.Apple , new TileStyleData(new SolidColorBrush(Colors.Red), "")},
        { TileType.DeliciousApple , new TileStyleData(new SolidColorBrush(Colors.DarkOrange), "")},
        { TileType.DisgustingApple , new TileStyleData(new SolidColorBrush(Colors.MediumPurple), "")},
        { TileType.RottenApple , new TileStyleData(new SolidColorBrush(Colors.DarkGreen), "")},
        { TileType.Body , new TileStyleData(new SolidColorBrush(Colors.GreenYellow), "")},
        { TileType.HeadLeft , new TileStyleData(new SolidColorBrush(Colors.GreenYellow),  ": ")},
        { TileType.HeadUp , new TileStyleData(new SolidColorBrush(Colors.GreenYellow), "' '")},
        { TileType.HeadRight , new TileStyleData(new SolidColorBrush(Colors.GreenYellow), " :")},
        { TileType.HeadDown , new TileStyleData(new SolidColorBrush(Colors.GreenYellow), ". .")}
    };
    
    private static readonly TileStyleData[] s_magicAppleStyles = new TileStyleData[] 
    {
        new TileStyleData(new SolidColorBrush(Colors.Pink), ""),
        new TileStyleData(new SolidColorBrush(Colors.CornflowerBlue), ""),
        new TileStyleData(new SolidColorBrush(Colors.Yellow), "")
    };

    private static readonly double s_timerInterval = 0.7d;
    private static readonly int s_appleInterval = 5;
    
    private static readonly string s_gameTitle = "Snake Game";

    private const int _size = 15;
    
    private DispatcherTimer _timer;
    private int _seconds;

    private DispatcherTimer _tickTimer;
    private int _ticks;
    
    private TextBlock[,] _renderedBoard;
    private Board _metaBoard;
    
    private Snake _snake;

    private CardinalDirection _snakeDirection;
    private bool _clicked;

    private List<Vector2Int> _positionsToAnimate;
    private int _animationTicks;
    
    public Root()
    {
        InitializeComponent();
        InitGame(null, null);
    }

    private void InitGame(object? sender, EventArgs? e)
    {
        //I know this is terrible, but don't worry, I didn't write it myself
        //I made python do it
        //anyways, this is tears and blood and I don't care enough to fix this*
        //it's the fault of this mf known as wpf because it doesn't allow to init widgets in the code behind
        //ok, it probably does, but I have no idea how to do it
        _renderedBoard = new TextBlock[_size, _size]
        {
            { Tb0_0, Tb0_1, Tb0_2, Tb0_3, Tb0_4, Tb0_5, Tb0_6, Tb0_7, Tb0_8, Tb0_9, Tb0_10, Tb0_11, Tb0_12, Tb0_13, Tb0_14 },
            { Tb1_0, Tb1_1, Tb1_2, Tb1_3, Tb1_4, Tb1_5, Tb1_6, Tb1_7, Tb1_8, Tb1_9, Tb1_10, Tb1_11, Tb1_12, Tb1_13, Tb1_14 },
            { Tb2_0, Tb2_1, Tb2_2, Tb2_3, Tb2_4, Tb2_5, Tb2_6, Tb2_7, Tb2_8, Tb2_9, Tb2_10, Tb2_11, Tb2_12, Tb2_13, Tb2_14 },
            { Tb3_0, Tb3_1, Tb3_2, Tb3_3, Tb3_4, Tb3_5, Tb3_6, Tb3_7, Tb3_8, Tb3_9, Tb3_10, Tb3_11, Tb3_12, Tb3_13, Tb3_14 },
            { Tb4_0, Tb4_1, Tb4_2, Tb4_3, Tb4_4, Tb4_5, Tb4_6, Tb4_7, Tb4_8, Tb4_9, Tb4_10, Tb4_11, Tb4_12, Tb4_13, Tb4_14 },
            { Tb5_0, Tb5_1, Tb5_2, Tb5_3, Tb5_4, Tb5_5, Tb5_6, Tb5_7, Tb5_8, Tb5_9, Tb5_10, Tb5_11, Tb5_12, Tb5_13, Tb5_14 },
            { Tb6_0, Tb6_1, Tb6_2, Tb6_3, Tb6_4, Tb6_5, Tb6_6, Tb6_7, Tb6_8, Tb6_9, Tb6_10, Tb6_11, Tb6_12, Tb6_13, Tb6_14 },
            { Tb7_0, Tb7_1, Tb7_2, Tb7_3, Tb7_4, Tb7_5, Tb7_6, Tb7_7, Tb7_8, Tb7_9, Tb7_10, Tb7_11, Tb7_12, Tb7_13, Tb7_14 },
            { Tb8_0, Tb8_1, Tb8_2, Tb8_3, Tb8_4, Tb8_5, Tb8_6, Tb8_7, Tb8_8, Tb8_9, Tb8_10, Tb8_11, Tb8_12, Tb8_13, Tb8_14 },
            { Tb9_0, Tb9_1, Tb9_2, Tb9_3, Tb9_4, Tb9_5, Tb9_6, Tb9_7, Tb9_8, Tb9_9, Tb9_10, Tb9_11, Tb9_12, Tb9_13, Tb9_14 },
            { Tb10_0, Tb10_1, Tb10_2, Tb10_3, Tb10_4, Tb10_5, Tb10_6, Tb10_7, Tb10_8, Tb10_9, Tb10_10, Tb10_11, Tb10_12, Tb10_13, Tb10_14 },
            { Tb11_0, Tb11_1, Tb11_2, Tb11_3, Tb11_4, Tb11_5, Tb11_6, Tb11_7, Tb11_8, Tb11_9, Tb11_10, Tb11_11, Tb11_12, Tb11_13, Tb11_14 },
            { Tb12_0, Tb12_1, Tb12_2, Tb12_3, Tb12_4, Tb12_5, Tb12_6, Tb12_7, Tb12_8, Tb12_9, Tb12_10, Tb12_11, Tb12_12, Tb12_13, Tb12_14 },
            { Tb13_0, Tb13_1, Tb13_2, Tb13_3, Tb13_4, Tb13_5, Tb13_6, Tb13_7, Tb13_8, Tb13_9, Tb13_10, Tb13_11, Tb13_12, Tb13_13, Tb13_14 },
            { Tb14_0, Tb14_1, Tb14_2, Tb14_3, Tb14_4, Tb14_5, Tb14_6, Tb14_7, Tb14_8, Tb14_9, Tb14_10, Tb14_11, Tb14_12, Tb14_13, Tb14_14 }
        };

        //*this too
        _metaBoard = Board.Of(new TileType[_size, _size]
        {
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.RottenApple ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.MagicApple ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
            { TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty ,TileType.Empty},
        });

        SnakeHead snakeHead = SnakeHead.Right;
        Vector2Int startingPosition = new Vector2Int(0, _metaBoard.Height / 2);
        
        _metaBoard.SetTile(startingPosition, TileType.Empty);
        
        PlaceStartingApple(startingPosition);

        _clicked = false;
        _snakeDirection = CardinalDirection.Of(snakeHead);
        _snake = new Snake(_metaBoard, startingPosition, snakeHead);

        InitTimer();
        InitTickTimer();
    }

    private void PlaceStartingApple(Vector2Int startingPosition)
    {
        Vector2Int applePosition;

        while (true)
        {
            applePosition = RandomPosition();

            if (!applePosition.Equals(startingPosition))
            {
                _metaBoard.AddApple(new Apple(applePosition));
                //_metaBoard.SetTile(applePosition, TileType.Apple);
                break;
            }
        }
    }

    private Vector2Int RandomPosition()
    {
        Random random = new Random((int) DateTime.Now.Ticks);
        
        return new Vector2Int(random.Next(0, _metaBoard.Height), random.Next(0, _metaBoard.Height));
    }

    private void InitTimer()
    {
        _seconds = 0;
        
        _timer = new DispatcherTimer();

        _timer.Tick += IncreaseTime;
        
        _timer.Interval = TimeSpan.FromSeconds(1d);
        _timer.Start();
    }
    
    private void IncreaseTime(object? sender, EventArgs e)
    {
        _seconds++;

        Title = $"{s_gameTitle} {GetTime(_seconds)}";
    }

    private string GetTime(int seconds)
    {
        int hours = seconds / 3600;
        
        seconds -= hours * 3600;
        
        int minutes = seconds / 60;
        
        seconds -= minutes * 60;

        return $"{hours}:{minutes:D2}:{seconds:D2}";
    }

    //starts the game loop
    //10 game ticks per second
    //hope this won't be laggy
    private void InitTickTimer()
    {
        _ticks = 0;
        
        _tickTimer = new DispatcherTimer();

        _tickTimer.Tick += UpdateBoard;
        _tickTimer.Tick += UpdateGameData;
        _tickTimer.Tick += RenderBoard;
        _tickTimer.Tick += IncreaseTicks;
        _tickTimer.Tick += (object? sender, EventArgs e) => { _clicked = false; };
        
        _tickTimer.Interval = TimeSpan.FromSeconds(s_timerInterval);
        _tickTimer.Start();
    }

    private void IncreaseTicks(object? sender, EventArgs? e) => _ticks++;

    private void UpdateGameData(object? sender, EventArgs e)
    {
        //Console.WriteLine(_snake.IsAlive);
        
        _tickTimer.Interval = TimeSpan.FromSeconds(s_timerInterval / (FloorDiv(_snake.Score, s_appleInterval) + 1));
        
        TbScore.Text = $"Score: {_snake.Score}";

        if (!_snake.IsAlive)
        {
            TbDeathMsg.Text = "Game over :(";
            
            _timer.Stop();
            _tickTimer.Stop();
            
            InitDeathAnimation();
        }
    }

    private void InitDeathAnimation()
    {
        _animationTicks = 0;
        
        _positionsToAnimate = new List<Vector2Int>();
        _positionsToAnimate.Add(ClosestInBoard(_snake.HeadPosition));

        _tickTimer.Tick -= UpdateBoard;
        _tickTimer.Tick -= UpdateGameData;
        _tickTimer.Tick -= IncreaseTicks;
        
        _tickTimer.Tick += PlayDeathAnimation;
        
        _tickTimer.Interval = TimeSpan.FromSeconds(0.2d);
        
        _tickTimer.Start();
    }

    private void PlayDeathAnimation(object? sender, EventArgs e)
    {
        _animationTicks++;
        
        //PrintDebugData();
        
        if (_animationTicks <= 1)
        {
            foreach (Vector2Int position in _positionsToAnimate)
            {
                _metaBoard.SetTile(position, TileType.Empty);
            }
            
            return;
        }

        if (_animationTicks > GetLongerBoardLength())
        {
            _tickTimer.Stop();
            return;
        }
        
        List<Vector2Int> newPositionsToAnimate = new List<Vector2Int>();
        
        foreach (Vector2Int position in _positionsToAnimate)
        {
            Vector2Int[] neighbors = GetNeighbors(position);

            foreach (Vector2Int neighbor in neighbors)
            {
                newPositionsToAnimate.Add(neighbor);
                
                _metaBoard.SetTile(neighbor, _metaBoard.GetTile(neighbor) == TileType.Body ? TileType.Empty : _metaBoard.GetTile(neighbor));
            }
        }
        
        _positionsToAnimate = newPositionsToAnimate;
    }

    private void PrintDebugData()
    {
        Console.WriteLine($"Animation Ticks: {_animationTicks}");
        
        Console.Write("[ |");
        
        foreach (Vector2Int position in _positionsToAnimate)
        {
            Console.Write($" ({position.X}, {position.Y}) {GetNeighbors(position).Length} |");
        }
        
        Console.WriteLine("]");
    }

    private void UpdateBoard(object? sender, EventArgs e)
    {
        if (_snake.IsAlive)
        {
            _snake.GoingDirection = _snakeDirection;
        
            _snake.Move();
        
            _metaBoard = _snake.Board;
        }
    }

    private void RenderBoard(object? sender, EventArgs e)
    {
        for (int i = 0; i < _metaBoard.Height * _metaBoard.Width; i++)
        {
            //y - row
            //x - column

            int y = i / _metaBoard.Width;
            int x = i % _metaBoard.Width;
                
            TileStyleData style;

            try
            {
                style = s_styles[_metaBoard.GetTile(y, x)];
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                style = s_styles[TileType.Empty];
                
                if (_metaBoard.GetTile(y, x) == TileType.MagicApple)
                {
                    style = s_magicAppleStyles[_ticks % s_magicAppleStyles.Length];
                }
            }
            
            _renderedBoard[y, x].Background = style.Background;
            _renderedBoard[y, x].Text = style.Text;
        }
    }

    private void Root_OnKeyDown(object sender, KeyEventArgs e)
    {
        //Console.WriteLine($"{e.Key}");
        
        CardinalDirection? direction = KeyToDirection(e.Key);

        if (direction != null && !_clicked)
        {
            _snakeDirection = direction != _snakeDirection.Opposite() ? direction : _snake.Body.Count > 1 ? _snakeDirection : direction;
        }
        
        _clicked = true;
    }

    private static CardinalDirection? KeyToDirection(Key key)
    {
        try
        {
            return s_directions[key];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
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

    private Vector2Int[] GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        
        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int neighbor = new Vector2Int(position.X + offset.X, position.Y + offset.Y);

            if (VectorIsInBoard(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors.ToArray();
    }

    private bool VectorIsInBoard(Vector2Int vector)
    {
        return vector.X < _metaBoard.Width
               && vector.Y < _metaBoard.Height
               && vector.X >= 0
               && vector.Y >= 0;;
    }

    private Vector2Int ClosestInBoard(Vector2Int vector)
    {
        vector.Y = vector.Y < 0 ? 0 : vector.Y;
        vector.Y = vector.Y >= _metaBoard.Height ? _metaBoard.Height - 1 : vector.Y;
        
        vector.X = vector.X < 0 ? 0 : vector.X;
        vector.X = vector.X >= _metaBoard.Width ? _metaBoard.Width - 1 : vector.X;
        
        return vector;
    }

    private int GetLongerBoardLength()
    {
        return _metaBoard.Height >= _metaBoard.Width ? _metaBoard.Height : _metaBoard.Width;
    }
}