using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using LEDControl.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using rpi_ws281x;

namespace LEDControl
{
    public class LEDEffects : ILEDEffects
    {
        private readonly ILEDState _ledState;
        private readonly ILogger<LEDEffects> _logger;

        private static double BrightnessPercentage => LEDControlData.strip.Brightness / 255.0;

        private Mode StateMode => _ledState.GetState().Mode;
        private LEDSettings? LEDSettings => _ledState.GetState().Settings;

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

            _logger.LogInformation($"State: {_ledState.GetState().Mode}");

            Clear();

            switch (StateMode)
            {
                case Mode.None: break;
                case Mode.Clear:
                    Clear();
                    break;
                case Mode.ColorWipe:
                    ColorWipe();
                    break;
                case Mode.StaticColor:
                    StaticColor();
                    break;
                case Mode.Rainbow:
                    Rainbow();
                    break;
                case Mode.RainbowCycle:
                    RainbowCycle();
                    break;
                case Mode.TheatreChase:
                    TheaterChase();
                    break;
                case Mode.TheatreChaseRainbow:
                    TheaterChaseRainbow();
                    break;
                case Mode.AppearFromBack:
                    AppearFromBack();
                    break;
                case Mode.Hyperspace:
                    Hyperspace();
                    break;
                case Mode.Breathing:
                    Breathe();
                    break;
                case Mode.BreathingRainbow:
                    BreathingRainbow();
                    break;
                case Mode.SelectedColors:
                    SelectedColors();
                    break;
                case Mode.Chaser:
                    Chaser();
                    break;
            }
        }

        #region Color Effects

        public void ColorWipe()
        {
            var color = GetColorAndApplyBrightness();

            using var rpi = new WS281x(LEDControlData.settings);

            do
            {
                Clear(rpi);
                
                for (var i = 0; i < LEDControlData.strip.LEDCount; i++)
                {
                    if (LEDState.IsDirty)
                        return;

                    LEDControlData.strip.SetLED(i, color);
                    rpi.Render();

                    Thread.Sleep(LEDSettings?.WaitTime ?? _defaultWaitTime);
                }
            } while (LEDSettings?.Loop ?? false);
        }

        public void StaticColor()
        {
            using var rpi = new WS281x(LEDControlData.settings);

            LEDControlData.strip.SetAll(GetColorAndApplyBrightness());
            rpi.Render();
        }

        public void Rainbow()
        {
            using var rpi = new WS281x(LEDControlData.settings);

            for (var i = 0; i < LEDControlData.strip.LEDCount; i++)
            {
                if (LEDState.IsDirty)
                    return;

                var color = Wheel(i & 255);
                LEDControlData.strip.SetLED(i, color);
            }

            rpi.Render();
        }

        public void RainbowCycle()
        {
            using var rpi = new WS281x(LEDControlData.settings);

            do
            {
                for (var j = 0; j < 256; j++)
                {
                    for (var i = 0; i < LEDControlData.strip.LEDCount; i++)
                    {
                        if (LEDState.IsDirty)
                            return;

                        var color = Wheel((i * 256 / LEDControlData.strip.LEDCount + j) & 255);
                        LEDControlData.strip.SetLED(i, color);
                    }

                    rpi.Render();
                    Thread.Sleep(LEDSettings?.WaitTime ?? _defaultWaitTime);
                }
            } while (LEDSettings?.Loop ?? false);
        }

        public void TheaterChase()
        {
            var color = GetColorAndApplyBrightness();

            using var rpi = new WS281x(LEDControlData.settings);

            do
            {
                for (var q = 0; q < 3; q++)
                {
                    for (var i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                    {
                        if (LEDState.IsDirty)
                            return;

                        LEDControlData.strip.SetLED(i + q, color);
                    }

                    rpi.Render();
                    Thread.Sleep(LEDSettings?.WaitTime ?? _defaultWaitTime);

                    for (var i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                    {
                        if (LEDState.IsDirty)
                            return;

                        LEDControlData.strip.SetLED(i + q, Color.Black);
                    }
                }
            } while (LEDSettings?.Loop ?? false);
        }

        public void TheaterChaseRainbow()
        {
            using var rpi = new WS281x(LEDControlData.settings);

            do
            {
                for (var q = 0; q < 3; q++)
                {
                    for (var i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                    {
                        if (LEDState.IsDirty)
                            return;

                        LEDControlData.strip.SetLED(i + q, Wheel((i) % 255));
                    }

                    rpi.Render();
                    Thread.Sleep(LEDSettings?.WaitTime ?? _defaultWaitTime);

                    for (var i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                    {
                        if (LEDState.IsDirty)
                            return;

                        LEDControlData.strip.SetLED(i + q, Color.Black);
                    }
                }
            } while (LEDSettings?.Loop ?? false);
        }

        public void AppearFromBack()
        {
            var color = GetColorAndApplyBrightness();

            using var rpi = new WS281x(LEDControlData.settings);

            do
            {
                for (var i = LEDControlData.strip.LEDCount - 1; i >= 0; i--)
                {
                    if (LEDState.IsDirty)
                        return;

                    LEDControlData.strip.SetLED(i, color);
                    rpi.Render();
                }
            } while (LEDSettings?.Loop ?? false);
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
            } while (LEDSettings?.Loop ?? false);
        }

        public void BreathingRainbow()
        {
            Color[] colors = new Color[]
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
            } while (LEDSettings?.Loop ?? false);
        }

        public void SelectedColors()
        {
            using var rpi = new WS281x(LEDControlData.settings);

            var ledIndex = 0;

            foreach (var key in LEDSettings?.Colors ?? new List<int>())
            {
                var color = Color.FromArgb(key);
                color = color.ApplyBrightnessToColor(BrightnessPercentage);

                LEDControlData.strip.SetLED(ledIndex, color);
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
            using var rpi = new WS281x(LEDControlData.settings);

            Clear(rpi, delay);
        }

        private static void Clear(WS281x rpi, int delay = 0)
        {
            if (delay < 0)
                delay = 0;

            if (delay == 0)
            {
                LEDControlData.strip.SetAll(Color.Black);
                rpi.Render();
            }
            else
            {
                for (var i = 0; i < LEDControlData.strip.LEDCount; i++)
                {
                    if (LEDState.IsDirty)
                        return;

                    LEDControlData.strip.SetLED(i, Color.Black);
                    rpi.Render();
                    Thread.Sleep(delay);
                }
            }
        }

        #endregion

        #region Private

        private Color GetColor()
        {
            return LEDSettings?.jsonColor.ToColor() ?? Color.Black;
        }

        private Color GetColorAndApplyBrightness()
        {
            return LEDSettings?.jsonColor.ApplyBrightnessToColor(BrightnessPercentage) ?? Color.Black;
        }

        private void Breathe(Color color)
        {
            using var rpi = new WS281x(LEDControlData.settings);
            
            for (var i = 0; i < LEDControlData.strip.LEDCount; i++)
            {
                if (LEDState.IsDirty)
                    return;

                var newColor = color.ApplyBrightnessToColor(Gaussian(i) / 255.0);
                
                LEDControlData.strip.SetAll(newColor);
                rpi.Render();
                
                Thread.Sleep(LEDSettings?.WaitTime ?? _defaultWaitTime);
            }
        }

        private static double Gaussian(int x)
        {
            const double alpha = 255.0;
            const double beta = 0.5;
            const double gamma = 0.14;
            double N = LEDControlData.strip.LEDCount;
            
            var top = -Math.Pow(x / N - beta, 2);
            var bottom = 2 * Math.Pow(gamma, 2);
            
            return alpha * Math.Exp(top / bottom);
        }

        private static Color Wheel(int pos)
        {
            switch (pos)
            {
                case < 85:
                    return Color.FromArgb(LEDControlData.strip.Brightness, pos * 3, 255 - pos * 3, 0);
                case < 170:
                    pos -= 85;
                    return Color.FromArgb(LEDControlData.strip.Brightness, 255 - pos * 3, 0, pos * 3);
                default:
                    pos -= 170;
                    return Color.FromArgb(LEDControlData.strip.Brightness, 0, pos * 3, 255 - pos * 3);
            }
        }
        
        #endregion
    }
}