namespace SnakeGame;

public class CardinalDirection
{
    
    //vertical coordinate is first
    public static readonly CardinalDirection Left = new CardinalDirection(new Vector2Int(-1, 0));
    public static readonly CardinalDirection Up = new CardinalDirection(new Vector2Int(0, -1));
    public static readonly CardinalDirection Right = new CardinalDirection(new Vector2Int(1, 0));
    public static readonly CardinalDirection Down = new CardinalDirection(new Vector2Int(0, 1));
    
    private static readonly Dictionary<SnakeHead, CardinalDirection> s_directions = new Dictionary<SnakeHead, CardinalDirection>()
    {
        { SnakeHead.Left , CardinalDirection.Left},
        { SnakeHead.Up , CardinalDirection.Up},
        { SnakeHead.Right , CardinalDirection.Right},
        { SnakeHead.Down , CardinalDirection.Down}
    };

    private static readonly Dictionary<CardinalDirection, CardinalDirection> s_oppositeDirections =
        new Dictionary<CardinalDirection, CardinalDirection>()
        {
            {CardinalDirection.Left, CardinalDirection.Right},
            {CardinalDirection.Up, CardinalDirection.Down},
            {CardinalDirection.Right, CardinalDirection.Left},
            {CardinalDirection.Down, CardinalDirection.Up}
        };
    
    private Vector2Int _vector;

    public Vector2Int Vector
    {
        get { return _vector; }
    }
    
    protected CardinalDirection(Vector2Int vector)
    {
        _vector = vector;
    }

    public CardinalDirection Opposite()
    {
        return s_oppositeDirections[this];
    }

    public static CardinalDirection Of(SnakeHead head)
    {
        return s_directions[head];
    }

    public static CardinalDirection OppositeOf(CardinalDirection direction)
    {
        return s_oppositeDirections[direction];
    }
}