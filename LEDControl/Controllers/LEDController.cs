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

        /// <summary>
        /// Sets the led strip color pixel by pixel by a certain wait time
        /// </summary>
        /// <param name="ledSettings"></param>
        [HttpPost("color_wipe")]
        public void ColorWipe(LEDSettings ledSettings)
        {
            SetState(Mode.ColorWipe, ledSettings);
        }
        
        /// <summary>
        /// Sets the strip a single color immediately
        /// </summary>
        /// <param name="ledSettings"></param>
        [HttpPost("static_color")]
        public void StaticColor(LEDSettings ledSettings)
        {
            SetState(Mode.StaticColor, ledSettings);
        }

        /// <summary>
        /// Runs the server to listen to the music visualizer client
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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

        private void SetState(Mode mode, LEDSettings ledSettings = null)
        {
            LEDState.IsDirty = true;
            
            _ledState.SetState(new LEDRequest
            {
                Mode = mode,
                Settings = ledSettings
            });
        }
    }
}
