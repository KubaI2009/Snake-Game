﻿using SnakeGame.apple;
using SnakeGame.snake;

namespace SnakeGame;

public class MagicApple : SpecialApple
{
    public MagicApple(int lifeTime, Vector2Int position) : base(lifeTime, position) { }

    public override TileType ToTileType()
    {
        return TileType.MagicApple;
    }

    public override int Points()
    {
        return 1;
    }
}