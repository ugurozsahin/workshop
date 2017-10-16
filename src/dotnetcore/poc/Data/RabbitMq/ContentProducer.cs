using Core.Config;
using Entity;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Data.RabbitMq
{
    public class ContentProducer : IContentProducer
    {
        private readonly IApplicationConfig _applicationConfig;

        public ContentProducer(IApplicationConfig applicationConfig)
        {
            _applicationConfig = applicationConfig;
        }

        public void Produce(Content content)
        {
            using (var connection = CreateConnection(_applicationConfig.ConnectionStrings.RabbitMq))
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "create-content",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                var message = JsonConvert.SerializeObject(content);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: string.Empty,
                                        routingKey: "create-content",
                                        basicProperties: null,
                                        body: body);
            }
        }

        private IConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("message", nameof(connectionString));
            }

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_applicationConfig.ConnectionStrings.RabbitMq)
            };

            var connection = factory.CreateConnection();
            return connection;
        }
    }

}
