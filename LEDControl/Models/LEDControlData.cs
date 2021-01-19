using rpi_ws281x;

namespace LEDControl.Models
{
    public class LEDControlData
    {
        public static bool isEnabled = false;

        public static Controller strip;
        public static Settings settings;

        static LEDControlData()
        {
            //The default settings uses a frequency of 800000 Hz and the DMA channel 10.
            settings = Settings.CreateDefaultSettings();

            //Set brightness to maximum (255)
            //Use Unknown as strip type. Then the type will be set in the native assembly.
            strip = settings.AddController(300, Pin.Gpio18, StripType.WS2812_STRIP, ControllerType.PWM0, 255, false);
        }
    }
}
