namespace SnakeGame.apple;

public abstract class SpecialApple : Apple
{
    private int _lifeTime;

    private int LifeTime
    {
        get { return _lifeTime; }
        set { _lifeTime = value < 0 ? 0 : value; }
    }
    
    protected SpecialApple(int lifeTime, Vector2Int position) : base(position)
    {
        _lifeTime = lifeTime < 0 ? 0 : lifeTime;
    }

    public bool IsAlive()
    {
        return _lifeTime > 0;
    }

    public void DecreaseLifeTime(int i)
    {
        LifeTime -= i;
    }

    public void Kill()
    {
        _lifeTime = 0;
    }
}