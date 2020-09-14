using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace LEDControl.Models
{
    public static class Extensions
    {
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
