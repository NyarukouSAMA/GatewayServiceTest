using MSP.QueueClient.Abstractions;
using MSP.QueueClient.Models.Options;
using RabbitMQ.Client;

namespace MSP.QueueClient
{
    public class RabbitMQConfigurator : IRabbitMQConfigurator
    {
        private readonly RabbitMQOptions _rabbitOptions;
        private readonly IRabbitMQConnectionProvider _connectionProvider;

        public RabbitMQConfigurator(RabbitMQOptions rabbitOptions,
            IRabbitMQConnectionProvider connectionProvider)
        {
            _rabbitOptions = rabbitOptions;
            _connectionProvider = connectionProvider;
        }

        public void Configure()
        {
            using (var channel = _connectionProvider.CreateChannelFromHostedConnection())
            {
                foreach (var exchange in _rabbitOptions.Exchanges)
                {
                    channel.ExchangeDeclare(exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete);
                }

                foreach (var queue in _rabbitOptions.Queues)
                {
                    channel.QueueDeclare(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
                }

                foreach (var binding in _rabbitOptions.Bindings)
                {
                    channel.QueueBind(binding.QueueName, binding.ExchangeName, binding.RoutingKey);
                }
            }
        }
    }
}
