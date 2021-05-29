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
                _ledState.SetState(new LEDRequest()
                {
                    Mode = Mode.Clear
                });
            }
            
            return Ok();
        }

        [HttpPost("color_wipe")]
        public void ColorWipe()
        {
            _ledState.SetState(new LEDRequest()
            {
                Mode = Mode.ColorWipe,
                Settings = new LEDSettings()
                {
                    
                }
            });
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
    }
}
