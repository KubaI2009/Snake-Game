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

    public void Subtract(Vector2Int v)
    {
        Add(ProductOfVectorAndScalar(-1, v));
    }

    public void MultiplyByScalar(int c)
    {
        X *= c;
        Y *= c;
    }

    public bool Equals(Vector2Int v)
    {
        return X == v.X && Y == v.Y;
    }

    public static Vector2Int SumOfVectors(Vector2Int v, Vector2Int u)
    {
        return new Vector2Int(v.X + u.X, v.Y + u.Y);
    }

    public static Vector2Int DifferenceOfVectors(Vector2Int v, Vector2Int u)
    {
        Vector2Int w = ProductOfVectorAndScalar(-1, u);
        
        return new Vector2Int(v.X + w.X, v.Y + w.Y);
    }

    public static Vector2Int ProductOfVectorAndScalar(int c, Vector2Int v)
    {
        return ProductOfVectorAndScalar(v, c);
    }

    public static int InnerProduct(Vector2Int v, Vector2Int u)
    {
        return v.X * u.Y + u.X * v.Y;
    }

    public static Vector2Int ProductOfVectorAndScalar(Vector2Int v, int c)
    {
        return new Vector2Int(v.X * c, v.Y * c);
    }

    public static bool VectorEquality(Vector2Int v, Vector2Int u)
    {
        return v.Equals(u);
    }

    public override string ToString()
    {
        return $"[{X}, {Y}]";
    }
}