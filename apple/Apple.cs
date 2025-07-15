using SnakeGame.snake;

namespace SnakeGame.apple;


public class Apple
{
    private Vector2Int _position;

    public int X
    {
        get { return _position.X; }
    }

    public int Y
    {
        get { return _position.Y; }
    }

    public Apple(Vector2Int position)
    {
        _position = position;
    }

    public virtual TileType ToTileType()
    {
        return TileType.Apple;
    }

    public virtual int Points()
    {
        return 1;
    }
}