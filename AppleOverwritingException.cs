namespace SnakeGame;

public class AppleOverwritingException : Exception
{
    public AppleOverwritingException() : base("An apple exists at the given location") { }
}