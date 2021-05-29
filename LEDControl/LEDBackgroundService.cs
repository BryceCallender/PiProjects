using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using LEDControl.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LEDControl
{
    public class LEDBackgroundService : BackgroundService
    {
        private readonly ILEDState _ledState;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundWorker> _logger;

        public LEDBackgroundService(ILEDState ledState, IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundWorker> logger)
        {
            _ledState = ledState;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Type} is now running in the background.", nameof(BackgroundWorker));

            await BackgroundProcessing(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical(
                "The {Type} is stopping due to a host shutdown, requests might not be processed anymore.",
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

                    if (_ledState.GetState().Mode == Mode.None) continue;
                    
                    if (LEDState.IsDirty)
                        _logger.LogInformation("LED change requested! Starting to process ..");

                    using var scope = _scopeFactory.CreateScope();
                    
                    var ledEffectService = scope.ServiceProvider.GetRequiredService<ILEDEffects>();

                    ledEffectService.HandleRequest();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("An error occurred when publishing a book. Exception: {@Exception}", ex);
                }
            }
        }
    }
}