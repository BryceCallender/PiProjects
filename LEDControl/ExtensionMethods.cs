using System;
using System.Drawing;
using LEDControl.Models;

namespace LEDControl
{
    public static class ExtensionMethods
    {
        public static Color FadeToBlackBy(this Color color, double amount)
        {
            return Color.FromArgb(LEDControlData.strip.Brightness, (int)(color.R * amount), (int)(color.G * amount), (int)(color.B * amount));
        }

        public static Color ApplyBrightnessToColor(this JsonColor color, double brightness)
        {
            return Color.FromArgb(LEDControlData.strip.Brightness, (int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }

        public static Color ApplyBrightnessToColor(this Color color, double brightness)
        {
            return Color.FromArgb(LEDControlData.strip.Brightness, (int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }
    }
}
