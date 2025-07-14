namespace SnakeGame.apple;

public abstract class SpecialApple : Apple
{
    private int _lifeTime;

    public int LifeTime
    {
        get { return _lifeTime; }
        private set { _lifeTime = value < 0 ? 0 : value; }
    }

    public bool IsAlive
    {
        get { return LifeTime > 0; }
    }
    
    protected SpecialApple(int lifeTime, Vector2Int position) : base(position)
    {
        _lifeTime = lifeTime < 0 ? 0 : lifeTime;
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