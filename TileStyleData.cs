using System.Windows.Media;

namespace SnakeGame;

public class TileStyleData
{
    private SolidColorBrush _background;
    private string _text;

    public SolidColorBrush Background
    {
        get { return _background; }
    }

    public string Text
    {
        get { return _text; }
    }
    
    public TileStyleData(SolidColorBrush background, string text)
    {
        _background = background;
        _text = text;
    }
}