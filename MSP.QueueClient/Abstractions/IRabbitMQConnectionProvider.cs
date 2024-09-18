using RabbitMQ.Client;

namespace MSP.QueueClient.Abstractions
{
    public interface IRabbitMQConnectionProvider
    {
        public IModel CreateChannelFromHostedConnection();
        public Task<IConnection> CreateNewConnectionAsync();
    }
}
