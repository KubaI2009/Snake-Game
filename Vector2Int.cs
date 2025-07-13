namespace SnakeGame;

public struct Vector2Int
{
    public int X { get; set; }

    public int Y { get; set; }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Add(Vector2Int v)
    {
        X += v.X;
        Y += v.Y;
    }

    public static Vector2Int SumOfVectors(Vector2Int v, Vector2Int u)
    {
        return new Vector2Int(v.X + u.X, v.Y + u.Y);
    }

    public bool Equals(Vector2Int v)
    {
        return X == v.X && Y == v.Y;
    }
}