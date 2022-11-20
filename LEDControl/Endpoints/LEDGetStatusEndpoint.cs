using FastEndpoints;
using LEDControl.Models;
using Microsoft.AspNetCore.Authorization;

namespace LEDControl.Endpoints;

[HttpGet("/api/led/status"), AllowAnonymous]
public class LEDGetStatusEndpoint : EndpointWithoutRequest<LEDStatus>
{
    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        await SendAsync(
            new LEDStatus { IsEnabled = LEDStrip.IsEnabled}, 
            cancellation: cancellationToken);
    }
}