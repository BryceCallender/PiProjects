using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using LEDControl.Models;
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
        private LEDSettings LEDSettings => _ledState.GetState().Settings;

        public LEDEffects(ILEDState ledState, ILogger<LEDEffects> logger)
        {
            _ledState = ledState;
            _logger = logger;
        }
        
        public void HandleRequest()
        {
            //State no longer dirty since we are processing it now
            //LEDState.IsDirty = false;
            
            _logger.LogInformation($"State: {_ledState.GetState().Mode}");

            switch (StateMode)
            {
                case Mode.None: break;
                case Mode.Clear: Clear();
                    break;
                case Mode.ColorWipe: ColorWipe();
                    break;
                case Mode.StaticColor: StaticColor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Color Effects
        
        public void ColorWipe()
        {
            var color = GetColorAndApplyBrightness();
            
            using var rpi = new WS281x(LEDControlData.settings);

            for (var i = 0; i < LEDControlData.strip.LEDCount; i++)
            {
                if (LEDState.IsDirty)
                    return;
                
                LEDControlData.strip.SetLED(i, color);
                rpi.Render();
            }
        }

        public void StaticColor()
        {
            using var rpi = new WS281x(LEDControlData.settings);
            
            LEDControlData.strip.SetAll(GetColorAndApplyBrightness());
            rpi.Render();
        }

        public void Rainbow()
        {
            throw new NotImplementedException();
        }

        public void RainbowCycle()
        {
            throw new NotImplementedException();
        }

        public void TheaterChase()
        {
            throw new NotImplementedException();
        }

        public void TheaterChaseRainbow()
        {
            throw new NotImplementedException();
        }

        public void AppearFromBack()
        {
            throw new NotImplementedException();
        }

        public void Hyperspace()
        {
            throw new NotImplementedException();
        }

        public void Breathe()
        {
            throw new NotImplementedException();
        }

        public void BreathingRainbow()
        {
            throw new NotImplementedException();
        }

        public void SelectedColors()
        {
            throw new NotImplementedException();
        }

        public void Chaser()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
        #region Private
        
        private Color GetColorAndApplyBrightness()
        {
            return LEDSettings.jsonColor.ApplyBrightnessToColor(BrightnessPercentage);
        }
        
        #endregion
    }
}