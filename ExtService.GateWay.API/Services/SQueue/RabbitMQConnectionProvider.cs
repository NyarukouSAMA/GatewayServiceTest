using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Models.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ExtService.GateWay.API.Services.SQueue
{
    public class RabbitMQConnectionProvider : IRabbitMQConnectionProvider, IDisposable
    {
        private readonly RabbitMQOptions _rabbitOptions;

        private readonly ConnectionFactory _connectionFactory;
        private Lazy<IConnection> _connection;

        public RabbitMQConnectionProvider(IOptions<RabbitMQOptions> rabbitOptions)
        {
            _rabbitOptions = rabbitOptions.Value;

            _connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitOptions.HostName,
                Port = _rabbitOptions.Port,
                UserName = _rabbitOptions.UserName,
                Password = _rabbitOptions.Password
            };

            InitializeRabbitMQObjects();

            _connection = new Lazy<IConnection>(() => _connectionFactory.CreateConnection());
        }

        private void InitializeRabbitMQObjects()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
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

        public async Task<TResponse> ExecuteChannelActionAsync<TResponse>(Func<IModel, TResponse> channelAction)
        {
            using (var channel = _connection.Value.CreateModel())
            {
                return await Task.Run(() => channelAction(channel));
            }
        }

        public void Dispose()
        {
            if (_connection.IsValueCreated)
            {
                _connection.Value.Dispose();
            }
        }

        
    }
}
