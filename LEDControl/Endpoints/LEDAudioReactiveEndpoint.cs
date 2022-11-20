using System.Diagnostics;
using FastEndpoints;
using LEDControl.Models;
using Microsoft.AspNetCore.Authorization;

namespace LEDControl.Endpoints;

[HttpPost("/api/led/audio_reactive"), AllowAnonymous]
public class LEDAudioReactiveEndpoint : Endpoint<AudioStatus, LEDResponse>
{
    private static Process? _visualizerProcess;
    
    public override Task HandleAsync(AudioStatus audioStatus, CancellationToken ct)
    {
        LEDStrip.IsEnabled = true;

         if (audioStatus.Enabled)
         {
             if (_visualizerProcess != null) 
                 return Task.CompletedTask;

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
             _visualizerProcess?.Kill();
             _visualizerProcess?.Dispose();
             _visualizerProcess = null;
         }

         return Task.CompletedTask;
    }
}