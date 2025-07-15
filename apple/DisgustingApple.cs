namespace SnakeGame.apple;

public class DisgustingApple : SpecialApple
{
    public DisgustingApple(int lifetime, Vector2Int position) : base(lifetime , position) { }

    public override TileType ToTileType()
    {
        return TileType.DisgustingApple;
    }

    public override int Points()
    {
        return -3;
    }
}