using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Niles.AI.Models;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using Niles.AI.Worker.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Niles.AI.Worker
{
    public class Worker : BackgroundService
    {
        private readonly NeuralNetworkRabbitMQService _neuralNetworkRabbitMQService;
        private readonly RabbitMQService _rabbitMQService;
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQConnectionOptions _settings;

        public Worker(
            NeuralNetworkRabbitMQService neuralNetworkRabbitMQService,
            RabbitMQService rabbitMQService,
            ILogger<Worker> logger,
            IOptions<RabbitMQConnectionOptions> settings
        )
        {
            _neuralNetworkRabbitMQService = neuralNetworkRabbitMQService ?? throw new ArgumentNullException(nameof(neuralNetworkRabbitMQService));
            _rabbitMQService = rabbitMQService ?? throw new ArgumentNullException(nameof(rabbitMQService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings.Value));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _neuralNetworkRabbitMQService.SubscribeOnGetInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnBuildInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnTrainInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnActivateInstanceQueue();
            _neuralNetworkRabbitMQService.SubscribeOnClearInstanceQueue();

            _neuralNetworkRabbitMQService.SendInstance();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
