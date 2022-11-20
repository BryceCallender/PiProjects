using System.Drawing;
using LEDControl.Models;

namespace LEDControl;

public static class ExtensionMethods
{
    public static Color FadeToBlackBy(this Color color, double amount)
    {
        return Color.FromArgb(LEDStrip.Brightness, (int)(color.R * amount), (int)(color.G * amount), (int)(color.B * amount));
    }

    public static Color ApplyBrightnessToColor(this Color color, double brightness)
    {
        return Color.FromArgb(LEDStrip.Brightness, (int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
    }
}
