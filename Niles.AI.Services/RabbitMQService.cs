using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Niles.AI.Models.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Niles.AI.Services
{
    ///<summary> Сервис для работы с RabbitMQ </summary>
    public class RabbitMQService
    {
        private readonly RabbitMQConnectionOptions _connectionSettings;
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(RabbitMQConnectionOptions connectionSettings, ILogger<RabbitMQService> logger)
        {
            _connectionSettings = connectionSettings ?? throw new ArgumentException(nameof(connectionSettings));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _connection = CreateConnection();
            _channel = CreateChannel();
        }

        ///<summary> Создание соединения </summary>
        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _connectionSettings.HostName,
                Port = _connectionSettings.Port,
                UserName = _connectionSettings.Username,
                Password = _connectionSettings.Password
            };

            return factory.CreateConnection();
        }

        ///<summary> Создание канала для подключения к очереди </summary>
        private IModel CreateChannel()
        {
            return _connection.CreateModel();
        }

        ///<summry> Подключение к очереди и отправка сообщения </summary>
        ///<param name="queueOptions"> Опции создания или подключения к очереди </param>
        ///<param name="message"> Сообщение, которое нужно передать </param>
        public void Send(RabbitMQQueueOptions queueOptions)
        {
            _logger.LogInformation($"Try to send to: {queueOptions.Name}");
            _channel.QueueDeclare(queue: queueOptions.Name,
                                 durable: queueOptions.Durable,
                                 exclusive: queueOptions.Exclusive,
                                 autoDelete: queueOptions.AutoDelete,
                                 arguments: queueOptions.Arguments);

            _channel.BasicPublish(exchange: "",
                                 routingKey: queueOptions.Name,
                                 basicProperties: null
                                 );
        }

        ///<summry> Подключение к очереди и отправка сообщения </summary>
        ///<param name="queueOptions"> Опции создания или подключения к очереди </param>
        ///<param name="message"> Сообщение, которое нужно передать </param>
        public void Send<T>(RabbitMQQueueOptions queueOptions, T message)
        {
            _logger.LogInformation($"Try to send to: {queueOptions.Name}");
            _channel.QueueDeclare(queue: queueOptions.Name,
                                 durable: queueOptions.Durable,
                                 exclusive: queueOptions.Exclusive,
                                 autoDelete: queueOptions.AutoDelete,
                                 arguments: queueOptions.Arguments);

            string serializedMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });
            var body = Encoding.UTF8.GetBytes(serializedMessage);

            _channel.BasicPublish(exchange: "",
                                 routingKey: queueOptions.Name,
                                 basicProperties: null,
                                 body: body
                                 );
            _logger.LogInformation($"Sent: {message}");
        }

        ///<summary> Подключение к очереди и подписка на получение сообщения </summary>
        ///<param name="options"> Опции создания или подключения к очереди </param>
        ///<param name="callback"> Обработчик события получения сообщения </param>
        public void Recieve(RabbitMQQueueOptions options)
        {
            _channel.QueueDeclare(options.Name,
                                options.Durable,
                                options.Exclusive,
                                options.AutoDelete,
                                options.Arguments);

            if (options.UseConsumerPrefetch)
            {
                _logger.LogInformation("Using consumer prefetch.");
                _logger.LogInformation($"Prefetch size: {options.PrefetchSize}");
                _logger.LogInformation($"Prefetch count: {options.PrefetchCount}");
                _logger.LogInformation($"Global: {options.Global}");

                _channel.BasicQos(options.PrefetchSize, options.PrefetchCount, options.Global);
            }

            _logger.LogInformation($"Waiting messages from {options.Name}");

            var consumer = new EventingBasicConsumer(_channel);
            if(options.OnReceive != null)
                consumer.Received += options.OnReceive;
            consumer.Received += (model, eventArgs) =>
            {
                _logger.LogInformation($"Received from: {options.Name}");
                if (!options.AutoAck)
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: options.Name,
                                 autoAck: options.AutoAck,
                                 consumer: consumer);
        }
    }
}