using Microsoft.Extensions.DependencyInjection;
using MSP.QueueClient.Abstractions;

namespace MSP.QueueClient
{
    /// <summary>
    /// This factory is implemented to use with AspNetCore DI container
    /// </summary>
    public class RabbitMQClientFactory : IRabbitMQClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRabbitMQConfigurator GetConfigurator()
        {
            return _serviceProvider.GetRequiredService<IRabbitMQConfigurator>();
        }

        public IRabbitMQConsumer GetConsumer<TData>()
        {
            return _serviceProvider.GetRequiredService<RabbitMQConsumer<TData>>();
        }

        public IRabbitMQProducer GetProducer()
        {
            return _serviceProvider.GetRequiredService<IRabbitMQProducer>();
        }
    }
}
