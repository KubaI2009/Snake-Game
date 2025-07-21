using SnakeGame.snake;

namespace SnakeGame.apple;

public class RottenApple : SpecialApple, IKillingApple
{
    public RottenApple(int lifeTime, Vector2Int position) : base(lifeTime, position) { }

    public override TileType ToTileType()
    {
        return TileType.RottenApple;
    }

    public override int Points()
    {
        return 0;
    }
}