using MSP.QueueClient.Abstractions;
using MSP.QueueClient.Models.Options;
using RabbitMQ.Client;

namespace MSP.QueueClient
{
    public class RabbitMQConnectionProvider : IRabbitMQConnectionProvider, IDisposable
    {
        private readonly RabbitMQOptions _rabbitOptions;

        private readonly ConnectionFactory _connectionFactory;
        private Lazy<IConnection> _connection;

        public RabbitMQConnectionProvider(RabbitMQOptions rabbitOptions)
        {
            _rabbitOptions = rabbitOptions;

            if (_rabbitOptions == null
                || string.IsNullOrWhiteSpace(_rabbitOptions.HostName)
                || string.IsNullOrWhiteSpace(_rabbitOptions.UserName)
                || string.IsNullOrWhiteSpace(_rabbitOptions.Password))
            {
                throw new ArgumentNullException("RabbitMQ options are not set correctly");
            }

            _connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitOptions.HostName,
                Port = _rabbitOptions.Port,
                UserName = _rabbitOptions.UserName,
                Password = _rabbitOptions.Password
            };

            _connection = new Lazy<IConnection>(() => _connectionFactory.CreateConnection());
        }

        public async Task<IConnection> CreateNewConnectionAsync()
        {
            return await Task.Run(() => _connectionFactory.CreateConnection());
        }

        public IModel CreateChannelFromHostedConnection()
        {
            return _connection.Value.CreateModel();
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
