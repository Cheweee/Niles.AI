using System;
using System.Collections.Generic;
using RabbitMQ.Client.Events;

namespace Niles.AI.Models.Settings
{
    public enum RabbitMQQueueNames {
        Train,
        Build,
        Activate,
        GetInstance,
        GetInstanceResponse,
        ClearInstance
    }

    ///<summary> Класс настроек подключения к RabbitMQ </summary>
    public class RabbitMQConnectionOptions
    {
        ///<summary> Адрес сервера </summary>
        public string HostName { get; set; }

        ///<summary> Виртуальный хост </summary>
        public string VHost { get; set; }

        ///<summary> Имя пользователя RabbitMQ </summary>
        public string Username { get; set; }

        ///<summary> Пароль пользователя RabbitMQ </summary>
        public string Password { get; set; }


        ///<summary> Порт сервера RabbitMQ </summary>
        public int Port { get; set; }
    }

    public class RabbitMQQueueOptions
    {
        ///<summary> Название очереди </summary>
        public string Name { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public bool AutoAck { get; set; } = true;
        public bool Global { get; set; }
        public bool UseConsumerPrefetch { get; set; }

        public uint PrefetchSize { get; set; } = 0;
        public ushort PrefetchCount { get; set; } = 1;

        ///<summary> Аргументы очереди </summary>
        public IDictionary<string, object> Arguments { get; set; }

        public EventHandler<BasicDeliverEventArgs> OnReceive;
    }
}
