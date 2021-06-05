using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LEDControl.Models;
using rpi_ws281x;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace LEDControl.Controllers
{
    [Route("api/[controller]")]
    public class LEDController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger _logger;

        private static Process _visualizerProcess;
        
        private readonly ILEDState _ledState;

        public LEDController(ILogger<LEDController> logger, ILEDState ledState)
        {
             _logger = logger;
             _ledState = ledState;
        }
        
        [HttpGet("led_status")]
        public IActionResult GetLEDStatus() => Ok(new { enabled = LEDControlData.IsEnabled });

        [HttpPost("change_status")]
        public IActionResult ChangeLEDStatus()
        {
            LEDControlData.IsEnabled = !LEDControlData.IsEnabled;
            
            if (!LEDControlData.IsEnabled)
            {
                SetState(Mode.Clear);
            }
            
            return Ok();
        }
        
        [HttpPost("color_wipe")]
        public void ColorWipe([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.ColorWipe, ledSettings);
        }
        
        [HttpPost("static_color")]
        public void StaticColor([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.StaticColor, ledSettings);
        }
        
        [HttpPost("rainbow")]
        public void Rainbow([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.Rainbow, ledSettings);
        }

        [HttpPost("rainbow_cycle")]
        public void RainbowCycle([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.RainbowCycle, ledSettings);
        }

        [HttpPost("theater_chase")]
        public void TheaterChase([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.TheatreChase, ledSettings);
        }

        [HttpPost("theater_chase_rainbow")]
        public void TheaterChaseRainbow([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.TheatreChaseRainbow, ledSettings);
        }

        [HttpPost("appear_from_back")]
        public void AppearFromBack([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.AppearFromBack, ledSettings);
        }

        [HttpPost("hyperspace")]
        public async Task Hyperspace([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.Hyperspace, ledSettings);
        }

        [HttpPost("breathing")]
        public void Breathe([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.Breathing, ledSettings);
        }

        [HttpPost("breathing_rainbow")]
        public void BreathingRainbow([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.BreathingRainbow, ledSettings);
        }

        [HttpPost("chaser")]
        public void Chaser([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.Chaser, ledSettings);
        }

        [HttpPost("selective_colors")]
        public void SelectedColors([FromBody] LEDSettings ledSettings)
        {
            SetState(Mode.SelectedColors, ledSettings);
        }

        [HttpPost("audio_reactive")]
        public IActionResult AudioReactiveLighting([FromBody] AudioStatus status)
        {
            LEDControlData.IsEnabled = true;

            if (status.Enabled)
            {
                if (_visualizerProcess != null) 
                    return Ok();

                _visualizerProcess = new Process
                {
                    StartInfo = new ProcessStartInfo("sudo", @" java -jar /home/pi/music-visualizer-server/dist/MusicVisualizerServer.jar")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };

                _visualizerProcess.Start();
            }
            else
            {
                _visualizerProcess.Kill();
                _visualizerProcess.Dispose();
                _visualizerProcess = null;
            }

            return Ok();
        }

        private void SetState(Mode mode, LEDSettings? ledSettings = null)
        {
            _ledState.SetState(new LEDRequest
            {
                Mode = mode,
                Settings = ledSettings
            });
        }
    }
}
