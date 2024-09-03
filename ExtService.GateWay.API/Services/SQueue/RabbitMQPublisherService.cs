using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ExtService.GateWay.API.Services.SQueue
{
    public class RabbitMQPublisherService : IRabbitMQPublisherService
    {
        private readonly ILogger<RabbitMQPublisherService> _logger;
        private readonly IRabbitMQConnectionProvider _connectionProvider;
        public RabbitMQPublisherService(ILogger<RabbitMQPublisherService> logger, 
            IRabbitMQConnectionProvider connectionProvider)
        {
            _logger = logger;
            _connectionProvider = connectionProvider;
        }

        public async Task<ServiceResponse<bool>> PublishToQueue<TMessage>(string exchangeName, string routingKey, TMessage message)
        {
            return await _connectionProvider.ExecuteChannelActionAsync(channel =>
            {
                try
                {
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: routingKey,
                        basicProperties: null,
                        body: body);

                    return new ServiceResponse<bool>
                    {
                        IsSuccess = true,
                        StatusCode = StatusCodes.Status200OK,
                        Data = true
                    };
                }
                catch (Exception ex)
                {
                    string headerMessage = $"Error while publishing message to queue: {routingKey}";

                    _logger.LogError(ex, headerMessage);

                    return new ServiceResponse<bool>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError,
                        ErrorMessage = headerMessage
                    };
                }
            });
        }
    }
}
