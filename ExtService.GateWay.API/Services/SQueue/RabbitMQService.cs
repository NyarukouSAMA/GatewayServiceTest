using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ExtService.GateWay.API.Services.SQueue
{
    public class RabbitMQService : IQueueService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly ConnectionFactory _connectionFactory;
        public RabbitMQService(ILogger<RabbitMQService> logger, ConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public Task<ServiceResponse<bool>> PublishToQueue<TMessage>(string exchangeName, string queueName, TMessage message)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: queueName,
                        basicProperties: null,
                        body: body);
                }

                return Task.FromResult(new ServiceResponse<bool> { IsSuccess = true });
            }
            catch (Exception ex)
            {
                string headerMessage = $"Error while publishing message to queue: {queueName}";

                _logger.LogError(ex, headerMessage);

                return Task.FromResult(new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                });
            }
        }

        public Task<ServiceResponse<bool>> PublishToQueue(string exchangeName, string queueName, string message)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: queueName,
                        basicProperties: null,
                        body: body);
                }

                return Task.FromResult(new ServiceResponse<bool> { IsSuccess = true });
            }
            catch (Exception ex)
            {
                string headerMessage = $"Error while publishing message to queue: {queueName}";

                _logger.LogError(ex, headerMessage);

                return Task.FromResult(new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                });
            }
        }
    }
}
