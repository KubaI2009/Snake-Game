using System;

//this ain't actually used but the name sounds cool so I kept it in the repo
//  \_'-'_/
public class SnakeOutOfBoundsException : Exception
{
    public SnakeOutOfBoundsException()
        : this("Snake must be inside of it's board")
    {
    }

    public SnakeOutOfBoundsException(string message)
        : base(message)
    {
    }

    public SnakeOutOfBoundsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}