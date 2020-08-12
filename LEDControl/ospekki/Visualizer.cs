using LEDControl.Models;
using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LEDControl.ospekki
{
    //This is not my own code it is a C# port of this instead of a Java server: 
    //https://github.com/ospekki/music-visualizer-server/blob/master/src/visualizer/Visualizer.java
    public class Visualizer
    {
        private static Settings settings;
        private static Controller strip;
        private static readonly Timer timer = new Timer();
        private static Server server;
        private static int[] data = new int[256];
        private static int[] spectrumData = new int[60];
        private static readonly float[] lineHeights = new float[60];
        private static readonly Color[] colors = new Color[60];
        private static readonly float[] colorVals = new float[60];
        private static double colorHue = 0.0;
        private static int ledCount = 60, effect = 0, colorM = 0, brightness = 255;
        private static int lineCount = 10, symmetric = 0;
        private static float speed1 = 0.20F, speed2 = 0.20F, lightTime = 5.0F;
        private static float minHeight = 50.0F;
        private static Color clr1 = Color.Yellow, clr2 = Color.White, clr3 = Color.Black;


        public void Start()
        {
            StartServer();
            Timer1();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.Empty;
            }
        }

        public void Stop()
        {
            server.StopServer();
        }

        static void StartServer()
        {
            try
            {
                server = new Server();
                server.Start();
                //This should mean server stopped. Free resources now
                //timer.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void SetLedColor(int i, Color clr)
        {
            if (symmetric == 0)
            {
                if (ledCount > 60 && effect != 2)
                {
                    int ledIdx = i * (ledCount / 60);
                    strip.SetLED(ledIdx, Color.FromArgb(255, clr.G, clr.R, clr.B));

                    for (int j = 1; j < (ledCount / 60); j++)
                    {
                        strip.SetLED(ledIdx + j, Color.FromArgb(255, clr.G, clr.R, clr.B));
                    }
                }
                else
                {
                    strip.SetLED(i, Color.FromArgb(255, clr.G, clr.R, clr.B));
                }
            }
            else
            {
                int lCount = ledCount / 2;
                if (lCount > 60 && effect != 2)
                {
                    int ledIdx = i * (lCount / 60);
                    strip.SetLED(ledIdx, Color.FromArgb(255, clr.G, clr.R, clr.B));

                    for (int j = 1; j < (lCount / 60); j++)
                    {
                        strip.SetLED(ledIdx + j, Color.FromArgb(255, clr.G, clr.R, clr.B));
                    }

                    int ledIdx2 = (ledCount - (i * (lCount / 60))) - 1;
                    strip.SetLED(ledIdx2, Color.FromArgb(255, clr.G, clr.R, clr.B));

                    for (int j = (lCount / 60) - 1; j >= 0; j--)
                    {
                        strip.SetLED(ledIdx2 - j, Color.FromArgb(255, clr.G, clr.R, clr.B));
                    }
                }
                else
                {
                    strip.SetLED(i, Color.FromArgb(255, clr.G, clr.R, clr.B));
                    strip.SetLED(ledCount - (i + 1), Color.FromArgb(255, clr.G, clr.R, clr.B));
                }
            }
        }

        // Sets new configurations received from the client
        static void SetConfigs(string configs)
        {
            string[] configArr = configs.Split("_");

            if (configArr.Length >= 18)
            {
                effect = int.Parse(configArr[0]);
                colorM = int.Parse(configArr[1]);
                minHeight = float.Parse(configArr[2]);
                lightTime = float.Parse(configArr[3]);
                speed1 = float.Parse(configArr[4]);
                speed2 = float.Parse(configArr[5]);
                brightness = int.Parse(configArr[6]);
                lineCount = int.Parse(configArr[7]);
                symmetric = int.Parse(configArr[8]);

                clr1 = Color.FromArgb(255, int.Parse(configArr[9]), int.Parse(configArr[10]),
                        int.Parse(configArr[11]));
                clr2 = Color.FromArgb(255, int.Parse(configArr[12]), int.Parse(configArr[13]),
                        int.Parse(configArr[14]));
                clr3 = Color.FromArgb(255, int.Parse(configArr[15]), int.Parse(configArr[16]),
                        int.Parse(configArr[17]));

                if (colorM == 0)
                {
                    colorHue = 0.0;
                }
            }
        }

        // Receives data from the client
        public static void GetData(byte[] bytes)
        {
            for (int j = 0; j < data.Length; j++)
            {
                int i = bytes[j];
                if (i < 0) { i = 256 + i; }
                data[j] = i;
            }

            int t = data[0];

            // Create an LED strip with the given settings
            if (t == 50)
            {
                if (strip == null)
                {
                    byte[] intBytes = new byte[bytes.Length - 1];

                    for (int i = 1; i < intBytes.Length; i++)
                    {
                        intBytes[i - 1] = bytes[i];
                    }

                    ledCount = int.Parse(new string(Encoding.Default.GetChars(intBytes)).Split("_")[0]);

                    //The default settings uses a frequency of 800000 Hz and the DMA channel 10.
                    settings = Settings.CreateDefaultSettings();

                    // new Ws281xLedStrip(led count, GPIO pin, frequench Hz, DMA, 
                    // brightness (0-255), PWM channel, invert, strip type, clear LEDs on exit)
                    //Use 16 LEDs and GPIO Pin 18.
                    //Set brightness to maximum (255)
                    //Use Unknown as strip type. Then the type will be set in the native assembly.
                    strip = settings.AddController(ledCount, Pin.Gpio18, StripType.WS2812_STRIP, ControllerType.PWM0, 255, false);
                }
            }

            // Sets new configurations
            if (t == 51)
            {
                byte[] configBytes = new byte[bytes.Length - 1];

                for (int i = 1; i < configBytes.Length; i++)
                {
                    configBytes[i - 1] = bytes[i];
                }

                SetConfigs(new string(Encoding.Default.GetChars(configBytes)));
            }

            if (t == 0 || t == 1 || t == 2)
            {
                if (strip != null)
                {
                    for (int i = 1; i < 61; i++)
                    {
                        spectrumData[i - 1] = data[i];
                    }
                }
            }
        }

        // The timer is used to call "BlinkingLeds", "DimmingLeds" and "Lines" 
        // methods every 10 milliseconds
        static void Timer1()
        {
            timer.Interval = 10;
            timer.Enabled = true;
            timer.Elapsed += (s, e) =>
            {
                if (strip != null)
                {
                    //using var rpi = new WS281x(settings);

                    if (effect == 0)
                    {
                        BlinkingLeds();
                    }

                    if (effect == 1)
                    {
                        DimmingLeds();
                    }

                    if (effect == 2)
                    {
                        Lines();
                    }

                    if (effect == 3)
                    {
                        Height();
                    }

                    if (colorM == 1)
                    {
                        colorHue += 0.15;
                    }

                    //rpi.Render();
                }
                
                Timer1();
            };
        }
        
    
        // Mixes two colors
        static Color Blend(Color colorA, Color colorB, float a)
        {
            if (a > 255.0F) { a = 255.0F; }
            float amount = 1.0F - (a / 255.0F);

            float r = ((colorA.R * amount) + colorB.R * (1 - amount));
            float g = ((colorA.G * amount) + colorB.G * (1 - amount));
            float b = ((colorA.B * amount) + colorB.B * (1 - amount));

            if (r < 0) { r = 0; }
            if (g < 0) { g = 0; }
            if (b < 0) { b = 0; }

            return  Color.FromArgb(255, (int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = (int)(Math.Round(Math.Floor(hue / 60)) % 6);
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            int v = (int)Math.Round(value);
            int p = (int)Math.Round(value * (1 - saturation));
            int q = (int)Math.Round(value * (1 - f * saturation));
            int t = (int)Math.Round(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }


        static Color GetColor(int idx)
        {
            Color clr = Color.Empty;
            colorVals[idx] = 255.0F;

            if (colorM == 0)
            {
                clr = Blend(clr1, clr2, spectrumData[idx] - minHeight);
            }

            if (colorM == 1)
            {
                clr = Blend(ColorFromHSV(colorHue, 1.0F, 1.0F),
                        ColorFromHSV(colorHue, 0.4F, 1.0F), spectrumData[idx] - minHeight);
            }

            return clr;
        }

        static void BlinkingLeds()
        {
            for (int i = 0; i < 60; i++)
            {
                if (spectrumData[i] > lineHeights[i])
                {
                    lineHeights[i] += speed1 * (spectrumData[i] - lineHeights[i]);

                    if (lineHeights[i] >= minHeight)
                    {
                        colors[i] = Blend(Color.Black, GetColor(i), brightness);
                        SetLedColor(i, colors[i]);
                    }
                }

                if (spectrumData[i] < lineHeights[i])
                {
                    lineHeights[i] -= speed2 * (lineHeights[i] - spectrumData[i]);

                    if (spectrumData[i] + lightTime < lineHeights[i] || lineHeights[i] <= 0.1F)
                    {
                        Color newClr = clr3;
                        colors[i] = Blend(Color.Black, newClr, brightness);
                        SetLedColor(i, colors[i]);
                    }
                }
            }
        }

        static void DimmingLeds()
        {
            for (int i = 0; i < 60; i++)
            {
                if (spectrumData[i] > lineHeights[i])
                {
                    lineHeights[i] += speed1 * (spectrumData[i] - lineHeights[i]);

                    if (lineHeights[i] >= minHeight)
                    {
                        colors[i] = Blend(Color.Black, GetColor(i), brightness);
                        colorVals[i] = brightness;
                        SetLedColor(i, colors[i]);
                    }
                }

                if (spectrumData[i] < lineHeights[i])
                {
                    lineHeights[i] -= speed2 * (lineHeights[i] - spectrumData[i]);

                    if (colorVals[i] > 0.0F)
                    {
                        if (spectrumData[i] + lightTime < lineHeights[i] || lineHeights[i] <= 0.1F)
                        {
                            colorVals[i] -= 20.0F * (brightness / 255.0F);

                            if (colors[i] != null)
                            {
                                Color color = Blend(clr3, colors[i], colorVals[i]);
                                SetLedColor(i, color);
                            }
                        }
                    }
                }
            }
        }

        static void Lines()
        {
            for (int i = 0; i < lineCount; i++)
            {
                int idx = i * (60 / lineCount);
                if (spectrumData[idx] > lineHeights[idx])
                {
                    lineHeights[idx] += speed1 * (spectrumData[idx] - lineHeights[idx]);
                }

                if (spectrumData[idx] < lineHeights[idx])
                {
                    lineHeights[idx] -= speed2 * (lineHeights[idx] - spectrumData[idx]);
                }

                int length;

                if (symmetric == 0)
                {
                    length = ledCount / lineCount;
                }
                else
                {
                    length = (ledCount / 2) / lineCount;
                }

                int startIdx = i * length;
                float lineHeight = lineHeights[idx] * ((float)length / 255.0F);

                for (int j = startIdx; j < (startIdx + length); j++)
                {
                    if ((j - startIdx) <= lineHeight)
                    {
                        SetLedColor(j, GetColor(idx));
                    }
                    else
                    {
                        SetLedColor(j, clr3);
                    }
                }
            }
        }

        static void Height()
        {
            double totalValues = 0;

            for(int i = 0; i < spectrumData.Length; i++)
            {
                totalValues += spectrumData[i];
            }

            int averageHeight = (int)Math.Round(totalValues / 256.0);

            for(int i = 0; i < averageHeight; i++)
            {
                colors[i] = Blend(Color.Black, GetColor(i), brightness);
                colorVals[i] = brightness;
                strip.SetLED(i, colors[i]);
            }

            for(int i = averageHeight; i < strip.LEDCount; i++)
            {
                strip.SetLED(i, Color.Black);
            }
        }
    }
}