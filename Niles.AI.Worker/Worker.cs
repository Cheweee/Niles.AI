using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using Niles.AI.Worker.Services;

namespace Niles.AI.Worker
{
    public class Worker : BackgroundService
    {
        private readonly WorkerRabbitMQService _neuralNetworkRabbitMQService;

        public Worker(
            WorkerRabbitMQService neuralNetworkRabbitMQService
        )
        {
            _neuralNetworkRabbitMQService = neuralNetworkRabbitMQService ?? throw new ArgumentNullException(nameof(neuralNetworkRabbitMQService));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _neuralNetworkRabbitMQService.SubscribeOnGetInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnBuildInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnTrainInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnActivateInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnClearInstanceQueue();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
