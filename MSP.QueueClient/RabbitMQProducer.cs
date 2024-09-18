using MSP.QueueClient.Abstractions;
using MSP.QueueClient.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MSP.QueueClient
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IRabbitMQConnectionProvider _connectionProvider;

        public RabbitMQProducer(IRabbitMQConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public async Task<Guid> PublishAsync<TData>(Message<TData> message, string exchangeName, string routingKey)
        {
            return await Task.Run(() =>
            {
                using (var channel = _connectionProvider.CreateChannelFromHostedConnection())
                {
                    channel.ConfirmSelect();

                    if (!Guid.TryParse(message.MessageId, out Guid messageId))
                    {
                        messageId = Guid.NewGuid();
                    }

                    var properties = channel.CreateBasicProperties();
                    properties.MessageId = messageId.ToString();
                    properties.DeliveryMode = 2;
                    properties.ContentType = "application/json";
                    properties.ContentEncoding = Encoding.UTF8.EncodingName;
                    properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                    if (!string.IsNullOrWhiteSpace(message.CorrelationId))
                    {
                        properties.CorrelationId = message.CorrelationId;
                    }

                    if (message.Headers?.Count > 0)
                    {
                        properties.Headers = new Dictionary<string, object>();

                        foreach (var header in message.Headers)
                        {
                            properties.Headers.Add(header.Key, header.Value);
                        }
                    }

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message.Data));

                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: routingKey,
                        basicProperties: properties,
                        body: body);

                    channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(10));

                    return messageId;
                }
            });
        }
    }
}
