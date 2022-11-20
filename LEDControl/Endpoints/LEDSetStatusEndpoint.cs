using FastEndpoints;
using LEDControl.Models;
using Microsoft.AspNetCore.Authorization;

namespace LEDControl.Endpoints;

[HttpPost("/api/led/status"), AllowAnonymous]
public class LEDSetStatusEndpoint : Endpoint<LEDStatus>
{
    private readonly ILEDState _ledState;
    
    public LEDSetStatusEndpoint(ILEDState ledState)
    {
        this._ledState = ledState;
    }
    
    public override Task HandleAsync(LEDStatus status, CancellationToken ct)
    {
        LEDStrip.IsEnabled = status.IsEnabled;

        if (!status.IsEnabled)
        {
            _ledState.SetState(new LEDRequest
            {
                Mode = LEDMode.Clear
            });
        }
        
        return Task.CompletedTask;
    }
}