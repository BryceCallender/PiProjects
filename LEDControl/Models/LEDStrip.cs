using System.Drawing;
using rpi_ws281x;

namespace LEDControl.Models;

public static class LEDStrip
{
    public static bool IsEnabled = false;

    private static readonly Controller Strip;
    public static readonly Settings Settings;

    public static int Brightness => Strip.Brightness;
    public static int Count => Strip.LEDCount;

    static LEDStrip()
    {
        //The default settings uses a frequency of 800000 Hz and the DMA channel 10.
        Settings = Settings.CreateDefaultSettings();

        //Set brightness to maximum (255)
        //Use Unknown as strip type. Then the type will be set in the native assembly.
        Strip = Settings.AddController(300, Pin.Gpio18, StripType.WS2812_STRIP);
    }

    public static void SetLED(int index, Color color)
    {
        Strip.SetLED(index, color);
    }

    public static void SetAll(Color color)
    {
        Strip.SetAll(color);
    }
}

