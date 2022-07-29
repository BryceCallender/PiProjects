using FastEndpoints;
using LEDControl.Models;
using Microsoft.AspNetCore.Authorization;

namespace LEDControl.Endpoints;

[HttpPost("/api/led/mode"), AllowAnonymous]
public class LEDModeEndpoint : Endpoint<LEDRequest, LEDResponse>
{
    private readonly ILEDState _ledState;
    
    public LEDModeEndpoint(ILEDState ledState)
    {
        this._ledState = ledState;
    }
    
    public override Task HandleAsync(LEDRequest request, CancellationToken cancellationToken)
    {
        _ledState.SetState(request);
        return Task.CompletedTask;
    }
}