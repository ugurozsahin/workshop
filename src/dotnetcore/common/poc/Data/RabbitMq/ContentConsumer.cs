using Core.Config;
using Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Data.RabbitMq
{
    public class ContentConsumer : IContentConsumer
    {
        private readonly IApplicationConfig _applicationConfig;
        private readonly ILogger<ContentConsumer> _logger;

        public ContentConsumer(
            IApplicationConfig applicationConfig,
            ILogger<ContentConsumer> logger)
        {
            _applicationConfig = applicationConfig;
            _logger = logger;
        }

        public void Consume()
        {
            _logger.LogInformation("Starting Consumption");

            using (var connection = CreateConnection(_applicationConfig.ConnectionStrings.RabbitMq))
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "create-content",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, basicDeliver) =>
                {
                    var body = basicDeliver.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var contentMessage = JsonConvert.DeserializeObject<Content>(message);
                    _logger.LogInformation(" [x] Received Creating content {0} {1}", contentMessage.Id, contentMessage.Title);
                };

                channel.BasicConsume("create-content", true, consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private IConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("connectionString", nameof(connectionString));
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
