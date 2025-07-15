using System.Windows.Media;

namespace SnakeGame;

public struct TileStyleData
{
    private SolidColorBrush _color;
    private string _text;

    public SolidColorBrush Color
    {
        get { return _color; }
    }

    public string Text
    {
        get { return _text; }
    }
    
    public TileStyleData(SolidColorBrush color, string text)
    {
        _color = color;
        _text = text;
    }
}