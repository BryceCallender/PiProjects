using FastEndpoints;
using LEDControl.Models;
using Microsoft.AspNetCore.Authorization;
using Mode = LEDControl.Models.Mode;

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
        LEDControlData.IsEnabled = status.IsEnabled;

        if (!status.IsEnabled)
        {
            _ledState.SetState(new LEDRequest
            {
                Mode = Mode.Clear
            });
        }
        
        return Task.CompletedTask;
    }
}