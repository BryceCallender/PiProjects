using System.ComponentModel;
using LEDControl.Models;

namespace LEDControl;

public class LEDBackgroundService : BackgroundService
{
    private readonly ILEDState _ledState;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LEDBackgroundService> _logger;

    public LEDBackgroundService(ILEDState ledState, IServiceScopeFactory scopeFactory, 
        ILogger<LEDBackgroundService> logger)
    {
        _ledState = ledState;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Type} is now running in the background", nameof(BackgroundWorker));

        await BackgroundProcessing(stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogCritical(
            "The {Type} is stopping due to a host shutdown, requests might not be processed anymore",
            nameof(BackgroundWorker)
        );

        return base.StopAsync(cancellationToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(500, stoppingToken);

                if (_ledState.State.Mode == LEDMode.None) 
                    continue;
                
                if (LEDState.IsDirty)
                    _logger.LogInformation("LED change requested! Starting to process...");

                using var scope = _scopeFactory.CreateScope();
                
                var ledEffectService = scope.ServiceProvider.GetRequiredService<ILEDEffects>();

                ledEffectService.HandleRequest();
                
                //Reset the state if the effect is a single use effect or non loopable 
                //and the state has not changed
                _ledState.ResetState();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("An error occurred when requesting a led change. Exception: {Exception}", ex);
            }
        }
    }
}