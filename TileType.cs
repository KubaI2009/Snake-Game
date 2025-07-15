using System.Windows.Media;

namespace SnakeGame;

public class TileType
{
    public static readonly TileType Empty = new TileType(new SolidColorBrush(Colors.Black));
    public static readonly TileType Apple = new TileType(new SolidColorBrush(Colors.Red));
    public static readonly TileType DeliciousApple = new TileType(new SolidColorBrush(Colors.DarkOrange));
    public static readonly TileType DisgustingApple = new TileType(new SolidColorBrush(Colors.MediumPurple));
    public static readonly TileType RottenApple = new TileType(new SolidColorBrush(Colors.DarkGreen));
    public static readonly TileType MagicApple = new TileType(new SolidColorBrush(Colors.Yellow));
    public static readonly TileType Body = new TileType(new SolidColorBrush(Colors.GreenYellow));
    public static readonly TileType HeadLeft = new TileType(new SolidColorBrush(Colors.GreenYellow),  ": ");
    public static readonly TileType HeadUp = new TileType(new SolidColorBrush(Colors.GreenYellow), "' '");
    public static readonly TileType HeadRight = new TileType(new SolidColorBrush(Colors.GreenYellow), " :");
    public static readonly TileType HeadDown = new TileType(new SolidColorBrush(Colors.GreenYellow), ". .");
    
    private TileStyleData _style;

    public TileStyleData Style
    {
        get { return _style; }
    }

    public SolidColorBrush color
    {
        get { return Style.Color; }
    }

    public string Text
    {
        get { return Style.Text; }
    }
    
    public TileType(SolidColorBrush color) : this(color, "") { }

    public TileType(SolidColorBrush color, string text) : this(new TileStyleData(color, text)) { }
    
    public TileType(TileStyleData style)
    {
        _style = style;
    }
}