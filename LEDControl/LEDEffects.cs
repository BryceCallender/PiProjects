using LEDControl.Models;
using rpi_ws281x;
using Color = System.Drawing.Color;

namespace LEDControl;

public class LEDEffects : ILEDEffects
{
    private readonly ILEDState _ledState;
    private readonly ILogger<LEDEffects> _logger;

    private static double BrightnessPercentage => LEDStrip.Brightness / 255.0;

    private LEDMode StateMode => _ledState.State.Mode;
    private LEDSettings? Settings => _ledState.State.Settings;

    private readonly int _defaultWaitTime;

    public LEDEffects(ILEDState ledState, ILogger<LEDEffects> logger, IConfiguration configuration)
    {
        _ledState = ledState;
        _logger = logger;

        _defaultWaitTime = configuration.GetValue<int>("LEDSettings:DefaultWaitTime");
    }

    public void HandleRequest()
    {
        LEDState.IsDirty = false;

        _logger.LogInformation("State: {Mode}", StateMode);

        Clear();

        switch (StateMode)
        {
            case LEDMode.None: break;
            case LEDMode.Clear:
                Clear();
                break;
            case LEDMode.ColorWipe:
                ColorWipe();
                break;
            case LEDMode.StaticColor:
                StaticColor();
                break;
            case LEDMode.Rainbow:
                Rainbow();
                break;
            case LEDMode.RainbowCycle:
                RainbowCycle();
                break;
            case LEDMode.TheatreChase:
                TheaterChase();
                break;
            case LEDMode.TheatreChaseRainbow:
                TheaterChaseRainbow();
                break;
            case LEDMode.AppearFromBack:
                AppearFromBack();
                break;
            case LEDMode.Hyperspace:
                Hyperspace();
                break;
            case LEDMode.Breathing:
                Breathe();
                break;
            case LEDMode.BreathingRainbow:
                BreathingRainbow();
                break;
            case LEDMode.SelectedColors:
                SelectedColors();
                break;
            case LEDMode.Chaser:
                Chaser();
                break;
        }
    }

    #region Color Effects

    public void ColorWipe()
    {
        var color = GetColorAndApplyBrightness();

        using var rpi = new WS281x(LEDStrip.Settings);

        do
        {
            Clear(rpi);
            
            for (var i = 0; i < LEDStrip.Count; i++)
            {
                if (LEDState.IsDirty)
                    return;

                LEDStrip.SetLED(i, color);
                rpi.Render();

                Thread.Sleep(Settings?.WaitTime ?? _defaultWaitTime);
            }
        } while (Settings?.Loop ?? false);
    }

    public void StaticColor()
    {
        using var rpi = new WS281x(LEDStrip.Settings);

        LEDStrip.SetAll(GetColorAndApplyBrightness());
        rpi.Render();
    }

    public void Rainbow()
    {
        using var rpi = new WS281x(LEDStrip.Settings);

        for (var i = 0; i < LEDStrip.Count; i++)
        {
            if (LEDState.IsDirty)
                return;

            var color = Wheel(i & 255);
            LEDStrip.SetLED(i, color);
        }

        rpi.Render();
    }

    public void RainbowCycle()
    {
        using var rpi = new WS281x(LEDStrip.Settings);

        do
        {
            for (var j = 0; j < 256; j++)
            {
                for (var i = 0; i < LEDStrip.Count; i++)
                {
                    if (LEDState.IsDirty)
                        return;

                    var color = Wheel((i * 256 / LEDStrip.Count + j) & 255);
                    LEDStrip.SetLED(i, color);
                }

                rpi.Render();
                Thread.Sleep(Settings?.WaitTime ?? _defaultWaitTime);
            }
        } while (Settings?.Loop ?? false);
    }

    public void TheaterChase()
    {
        var color = GetColorAndApplyBrightness();

        using var rpi = new WS281x(LEDStrip.Settings);

        do
        {
            for (var q = 0; q < 3; q++)
            {
                for (var i = 0; i < LEDStrip.Count; i += 3)
                {
                    if (LEDState.IsDirty)
                        return;

                    LEDStrip.SetLED(i + q, color);
                }

                rpi.Render();
                Thread.Sleep(Settings?.WaitTime ?? _defaultWaitTime);

                for (var i = 0; i < LEDStrip.Count; i += 3)
                {
                    if (LEDState.IsDirty)
                        return;

                    LEDStrip.SetLED(i + q, Color.Black);
                }
            }
        } while (Settings?.Loop ?? false);
    }

    public void TheaterChaseRainbow()
    {
        using var rpi = new WS281x(LEDStrip.Settings);

        do
        {
            for (var q = 0; q < 3; q++)
            {
                for (var i = 0; i < LEDStrip.Count; i += 3)
                {
                    if (LEDState.IsDirty)
                        return;

                    LEDStrip.SetLED(i + q, Wheel((i) % 255));
                }

                rpi.Render();
                Thread.Sleep(Settings?.WaitTime ?? _defaultWaitTime);

                for (var i = 0; i < LEDStrip.Count; i += 3)
                {
                    if (LEDState.IsDirty)
                        return;

                    LEDStrip.SetLED(i + q, Color.Black);
                }
            }
        } while (Settings?.Loop ?? false);
    }

    public void AppearFromBack()
    {
        var color = GetColorAndApplyBrightness();

        using var rpi = new WS281x(LEDStrip.Settings);

        do
        {
            for (var i = LEDStrip.Count - 1; i >= 0; i--)
            {
                if (LEDState.IsDirty)
                    return;

                LEDStrip.SetLED(i, color);
                rpi.Render();
            }
        } while (Settings?.Loop ?? false);
    }

    public void Hyperspace()
    {
        throw new NotImplementedException();
    }

    public void Breathe()
    {
        do
        {
            if (LEDState.IsDirty)
                return;
            
            Breathe(GetColor());
        } while (Settings?.Loop ?? false);
    }

    public void BreathingRainbow()
    {
        var colors = new Color[]
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.GreenYellow,
            Color.Blue,
            Color.MediumPurple,
            Color.Pink
        };

        do
        {
            foreach (var color in colors)
            {
                if (LEDState.IsDirty)
                    return;

                Breathe(color);
            }
        } while (Settings?.Loop ?? false);
    }

    public void SelectedColors()
    {
        using var rpi = new WS281x(LEDStrip.Settings);

        var ledIndex = 0;

        foreach (var color in Settings?.Colors)
        {
            var newColor = color.ApplyBrightnessToColor(BrightnessPercentage);

            LEDStrip.SetLED(ledIndex, newColor);
            ledIndex++;
        }

        rpi.Render();
    }

    public void Chaser()
    {
        throw new NotImplementedException();
    }

    public void Clear(int delay = 0)
    {
        using var rpi = new WS281x(LEDStrip.Settings);

        Clear(rpi, delay);
    }
    
    private static void Clear(WS281x rpi, int delay = 0)
    {
        if (delay < 0)
            delay = 0;

        if (delay == 0)
        {
            LEDStrip.SetAll(Color.Black);
            rpi.Render();
        }
        else
        {
            for (var i = 0; i < LEDStrip.Count; i++)
            {
                if (LEDState.IsDirty)
                    return;

                LEDStrip.SetLED(i, Color.Black);
                rpi.Render();
                Thread.Sleep(delay);
            }
        }
    }

    #endregion

    #region Private

    private Color GetColor()
    {
        return Settings?.Color ?? Color.Black;
    }

    private Color GetColorAndApplyBrightness()
    {
        return Settings?.Color?.ApplyBrightnessToColor(BrightnessPercentage) ?? Color.Black;
    }

    private void Breathe(Color color)
    {
        using var rpi = new WS281x(LEDStrip.Settings);
        
        for (var i = 0; i < LEDStrip.Count; i++)
        {
            if (LEDState.IsDirty)
                return;

            var newColor = color.ApplyBrightnessToColor(Gaussian(i) / 255.0);
            
            LEDStrip.SetAll(newColor);
            rpi.Render();
            
            Thread.Sleep(Settings?.WaitTime ?? _defaultWaitTime);
        }
    }

    private static double Gaussian(int x)
    {
        const double alpha = 255.0;
        const double beta = 0.5;
        const double gamma = 0.14;
        double N = LEDStrip.Count;
        
        var top = -Math.Pow(x / N - beta, 2);
        var bottom = 2 * Math.Pow(gamma, 2);
        
        return alpha * Math.Exp(top / bottom);
    }

    private static Color Wheel(int pos)
    {
        switch (pos)
        {
            case < 85:
                return Color.FromArgb(LEDStrip.Brightness, pos * 3, 255 - pos * 3, 0);
            case < 170:
                pos -= 85;
                return Color.FromArgb(LEDStrip.Brightness, 255 - pos * 3, 0, pos * 3);
            default:
                pos -= 170;
                return Color.FromArgb(LEDStrip.Brightness, 0, pos * 3, 255 - pos * 3);
        }
    }
    
    #endregion
}