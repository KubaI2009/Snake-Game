using SnakeGame.snake;

namespace SnakeGame.apple;

public class RottenApple : SpecialApple
{
    public RottenApple(int lifeTime, Vector2Int position) : base(lifeTime, position) { }

    public override void ApplyEffectToSnake(Snake snake)
    {
        snake.AteRottenApple = true;
    }

    public override TileType ToTileType()
    {
        return TileType.RottenApple;
    }
}