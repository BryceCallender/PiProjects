using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LEDControl.Models;
using rpi_ws281x;
using System.Drawing;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LEDControl.Controllers
{
    [Route("api/[controller]")]
    public class LEDController : Microsoft.AspNetCore.Mvc.Controller
    {
        // GET api/<controller>/led_status
        [HttpGet("led_status")]
        public string GetLEDStatus()
        {
            return JsonConvert.SerializeObject(new { enabled = LEDControlData.isEnabled });
        }

        // POST api/<controller>/change_status
        [HttpPost("change_status")]
        public void ChangeLEDStatus()
        {
            LEDControlData.isEnabled = !LEDControlData.isEnabled;

            if(!LEDControlData.isEnabled)
            {
                using (var rpi = new WS281x(LEDControlData.settings))
                {
                    LEDControlData.strip.SetAll(Color.Black);
                    rpi.Render();
                }
            }
        }

        [HttpPost("color_wipe")]
        public void ColorWipe([FromBody] JsonColor jsonColor, int waitTime = 50)
        {
            Color color = Color.FromArgb(255, jsonColor.R, jsonColor.G, jsonColor.B);

            Debug.WriteLine(color);

            using (var rpi = new WS281x(LEDControlData.settings))
            {
                for(int i = 0; i < LEDControlData.strip.LEDCount; i++)
                {
                    LEDControlData.strip.SetLED(i, color);
                    rpi.Render();

                    Thread.Sleep(waitTime);
                }
            }
        }

        [HttpPost("static_color")]
        public void StaticColor([FromBody] JsonColor jsonColor)
        {
            Color color = Color.FromArgb(LEDControlData.strip.Brightness, jsonColor.R, jsonColor.G, jsonColor.B);

            using (var rpi = new WS281x(LEDControlData.settings))
            {
                LEDControlData.strip.SetAll(color);
                rpi.Render();
            }
        }

        [HttpPost("rainbow")]
        public void Rainbow(int waitTime = 20, int iterations = 1)
        {
            using (var rpi = new WS281x(LEDControlData.settings))
            {
                for (int j = 0; j < 256 * iterations; j++)
                {
                    for (int i = 0; i < LEDControlData.strip.LEDCount; i++)
                    {
                        Color color = Wheel((i + j) & 255);
                        LEDControlData.strip.SetLED(i, color);
                    }

                    rpi.Render();
                    Thread.Sleep(waitTime);
                }
            }
        }

        [HttpPost("rainbow_cycle")]
        public void RainbowCycle(int waitTime = 20, int iterations = 5)
        {
            using (var rpi = new WS281x(LEDControlData.settings))
            {
                for (int j = 0; j < 256 * iterations; j++)
                {
                    for (int i = 0; i < LEDControlData.strip.LEDCount; i++)
                    {
                        Color color = Wheel(((i * 256 / LEDControlData.strip.LEDCount) + j) & 255);
                        LEDControlData.strip.SetLED(i, color);
                    }

                    rpi.Render();
                    Thread.Sleep(waitTime);
                }
            }
        }

        [HttpPost("theater_chase")]
        public void TheaterChase([FromBody] JsonColor jsonColor, int waitTime = 50, int iterations = 5)
        {
            Color color = Color.FromArgb(LEDControlData.strip.Brightness, jsonColor.R, jsonColor.G, jsonColor.B);

            using (var rpi = new WS281x(LEDControlData.settings))
            {
                for (int j = 0; j < iterations; j++)
                {
                    for (int q = 0; q < 3; q++)
                    {
                        for (int i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                        {
                            LEDControlData.strip.SetLED(i + q, color);
                        }

                        rpi.Render();
                        Thread.Sleep(waitTime);

                        for(int i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                        {
                            LEDControlData.strip.SetLED(i + q, Color.Black);
                        }
                    }
                }
            }
        }

        [HttpPost("theater_chase_rainbow")]
        public void TheaterChaseRainbow(int waitTime = 50, int iterations = 5)
        {
            using (var rpi = new WS281x(LEDControlData.settings))
            {
                for (int j = 0; j < iterations; j++)
                {
                    for (int q = 0; q < 3; q++)
                    {
                        for (int i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                        {
                            LEDControlData.strip.SetLED(i + q, Wheel((i + j) % 255));
                        }

                        rpi.Render();
                        Thread.Sleep(waitTime);

                        for (int i = 0; i < LEDControlData.strip.LEDCount; i += 3)
                        {
                            LEDControlData.strip.SetLED(i + q, Color.Black);
                        }
                    }
                }
            }
        }

        [HttpPost("appear_from_back")]
        public void AppearFromBack([FromBody] JsonColor jsonColor, int waitTime = 50)
        {
            for(int j = LEDControlData.strip.LEDCount - 1; j >= 0; j--)
            {
                //for(int i = 0; i < )
            }
        }

        [HttpPost("hyperspace")]
        public void Hyperspace(int length = 5)
        {
            using (var rpi = new WS281x(LEDControlData.settings))
            {
                for (int i = 0; i < LEDControlData.strip.LEDCount; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if (i + j < LEDControlData.strip.LEDCount)
                        {
                            LEDControlData.strip.SetLED(i + j, Color.White);
                            rpi.Render();
                        }
                    }

                    LEDControlData.strip.SetLED(i, Color.Black);
                }

                LEDControlData.strip.SetLED(LEDControlData.strip.LEDCount - 1, Color.Black);
            }
        }


        private Color Wheel(int pos)
        {
            if (pos < 85)
            {
                return Color.FromArgb(LEDControlData.strip.Brightness, pos * 3, 255 - pos * 3, 0);
            }
            else if (pos < 170)
            {
                pos -= 85;
                return Color.FromArgb(LEDControlData.strip.Brightness, 255 - pos * 3, 0, pos * 3);
            }
            else
            {
                pos -= 170;
                return Color.FromArgb(LEDControlData.strip.Brightness, 0, pos * 3, 255 - pos * 3);
            }
        }
        

    }
}
