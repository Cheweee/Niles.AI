using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Niles.AI.Models.Settings;
using Niles.AI.Services;
using RabbitMQ.Client.Events;

namespace Niles.AI.Worker.Services
{
    public class NeuralNetworkRabbitMQService
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly NeuralNetworkService _neuralNetworkService;

        public NeuralNetworkRabbitMQService(
            RabbitMQService rabbitMQService,
            NeuralNetworkService neuralNetworkService
        )
        {
            _rabbitMQService = rabbitMQService ?? throw new ArgumentNullException(nameof(rabbitMQService));
            _neuralNetworkService = neuralNetworkService ?? throw new ArgumentNullException(nameof(neuralNetworkService));
        }

        public void SubscribeOnGetInstanceQueue()
        {
            _rabbitMQService.Recieve(new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.GetInstance.ToString(),
                OnReceive = GetIstanceResponse
            });
        }

        public void SubscribeOnBuildInstanceQueue()
        {
            _rabbitMQService.Recieve(new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.Build.ToString(),
                OnReceive = BuildInstanceResponse
            });
        }

        public void SubscribeOnTrainInstanceQueue()
        {
            _rabbitMQService.Recieve(new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.Train.ToString(),
                OnReceive = TrainInstanceResponse
            });
        }

        public void SubscribeOnActivateInstanceQueue()
        {
            _rabbitMQService.Recieve(new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.Activate.ToString(),
                OnReceive = ActivateInstanceResponse
            });
        }

        public void SubscribeOnClearInstanceQueue()
        {
            _rabbitMQService.Recieve(new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.ClearInstance.ToString(),
                OnReceive = ClearInstanceResponse
            });
        }

        ///<summary> Ответ на событие очереди Build </summary>
        private void BuildInstanceResponse(object sender, BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body.ToArray());
            var options = JsonConvert.DeserializeObject<NeuralNetworkBuildOptions>(message);

            _neuralNetworkService.InitializeNetworkInstance(options);

            SendInstance();
        }

        ///<summary> Ответ на событие очереди Train </summary>
        private void TrainInstanceResponse(object sender, BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body.ToArray());
            var options = JsonConvert.DeserializeObject<NeuralNetworkTrainOptions>(message);

            _neuralNetworkService.Train(options);

            SendInstance();
        }

        ///<summary> Ответ на событие очереди Activate </summary>
        private void ActivateInstanceResponse(object sender, BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body.ToArray());
            var options = JsonConvert.DeserializeObject<NeuralNetworkActivateOptions>(message);

            _neuralNetworkService.Activate(options);

            SendInstance();
        }

        ///<summary> Ответ на событие очереди GetInstance </summary>
        private void GetIstanceResponse(object sender, BasicDeliverEventArgs eventArgs)
        {
            SendInstance();
        }

        ///<summary> Очищает текущую структуру нейронной сети </summary>
        private void ClearInstanceResponse(object sender, BasicDeliverEventArgs eventArgs)
        {
            _neuralNetworkService.ClearNetworkInstance();

            SendInstance();
        }

        ///<summary> Возвращает текущую структуру нейронной сети </summary>
        public void SendInstance()
        {
            _rabbitMQService.Send(new RabbitMQQueueOptions
            {
                AutoAck = true,
                AutoDelete = false,
                Durable = false,
                Exclusive = false,
                Global = false,
                Name = RabbitMQQueueNames.GetInstanceResponse.ToString()
            }, _neuralNetworkService.Instance);
        }
    }
}