using MSP.QueueClient.Models;

namespace MSP.QueueClient.Abstractions
{
    public interface IRabbitMQProducer
    {
        public Task<Guid> PublishAsync<TData>(Message<TData> message, string exchangeName, string routingKey);
    }
}
