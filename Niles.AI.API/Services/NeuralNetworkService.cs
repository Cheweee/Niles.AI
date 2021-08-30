using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Niles.AI.API.Hubs;
using Niles.AI.Models;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using Niles.AI.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Niles.AI.API.Services
{
    public class NeuralNetworkService : INeuralNetworkService
    {        
        private readonly ILogger<NeuralNetworkService> _logger;
        private readonly RabbitMQService _rabbitMQService;
        private readonly NeuralNetworkHub _hub;

        public NeuralNetworkService(RabbitMQService rabbitMQService, NeuralNetworkHub hub, ILogger<NeuralNetworkService> logger)
        {
            _rabbitMQService = rabbitMQService ?? throw new ArgumentNullException(nameof(rabbitMQService));
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        ///<summary> Обращаемся к worker'у, чтобы он вернул текущую структуру нейронной сети </summary>
        public void GetInstance()
        {
            var options = new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.GetInstance.ToString()
            };

            _rabbitMQService.Send(options);
        }

        ///<summary> Обращаемся к worker'у, чтобы он активировал текущую нейронную сеть </summary>
        public void Activate(NeuralNetworkActivateOptions options)
        {
            var settings = new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.Activate.ToString(),
            };
            _rabbitMQService.Send(settings, options);
        }

        public void Build(NeuralNetworkBuildOptions options)
        {
            var settings = new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.Build.ToString(),
            };
            _rabbitMQService.Send(settings, options);
        }

        public void Train(NeuralNetworkTrainOptions options)
        {
            var settings = new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.Train.ToString(),
            };
            _rabbitMQService.Send(settings, options);
        }

        public void ClearInstance()
        {
            var settings = new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.ClearInstance.ToString(),
            };
            _rabbitMQService.Send(settings);
        }

        public void SubscribeOnGetInstanceResponse()
        {
            _logger.LogInformation($"Try to subscribe on \"{RabbitMQQueueNames.GetInstanceResponse}\" queue");
            var settings = new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.GetInstanceResponse.ToString(),
                OnReceive = GetInstanceCallback
            };
            _rabbitMQService.Recieve(settings);
        }

        private async void GetInstanceCallback(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body.ToArray());
                var instance = JsonConvert.DeserializeObject<NeuralNetwork>(message);

                await _hub.Send(message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }

        }
    }
}
