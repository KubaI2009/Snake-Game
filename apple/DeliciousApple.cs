namespace SnakeGame.apple;

public class DeliciousApple : Apple
{
    public DeliciousApple(Vector2Int position) : base(position) { }

    public override TileType ToTileType()
    {
        return TileType.DeliciousApple;
    }
}