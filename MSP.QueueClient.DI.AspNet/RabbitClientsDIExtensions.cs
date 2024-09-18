using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MSP.QueueClient.Abstractions;
using MSP.QueueClient.Models.Options;

namespace MSP.QueueClient.DI.AspNet
{
    public static class RabbitClientsDIExtensions
    {
        public static IServiceCollection AddRabbitMQSingletonConsumer<TData>(this IServiceCollection services, 
            string queueName,
            Action<Exception> errorHandler = null)
        {
            services.TryAddSingleton(provider => new RabbitMQConnectionProvider(
                provider.GetRequiredService<IOptions<RabbitMQOptions>>().Value));
            services.TryAddSingleton<IRabbitMQConnectionProvider>(provider => provider.GetRequiredService<RabbitMQConnectionProvider>());
            services.TryAddSingleton<IRabbitMQClientFactory, RabbitMQClientFactory>();
            services.TryAddSingleton(provider =>
            {
                var consumer = new RabbitMQConsumer<TData>(
                    queueName,
                    provider.GetRequiredService<IRabbitMQConnectionProvider>(),
                    provider.GetRequiredService<IMediator>());

                consumer.ErrorHandler = errorHandler;

                return consumer;
            });

            return services;
        }

        public static IServiceCollection AddRabbitMQSingletonProducer(this IServiceCollection services)
        {
            services.TryAddSingleton(provider => new RabbitMQConnectionProvider(
                provider.GetRequiredService<IOptions<RabbitMQOptions>>().Value));
            services.TryAddSingleton<IRabbitMQConnectionProvider>(provider => provider.GetRequiredService<RabbitMQConnectionProvider>());
            services.TryAddSingleton<IRabbitMQClientFactory, RabbitMQClientFactory>();
            services.TryAddSingleton<IRabbitMQProducer>(provider => new RabbitMQProducer(
                provider.GetRequiredService<IRabbitMQConnectionProvider>()));

            return services;
        }

        public static IServiceCollection AddRabbitMQSingletonConfigurator(this IServiceCollection services)
        {
            services.TryAddSingleton(provider => new RabbitMQConnectionProvider(
                provider.GetRequiredService<IOptions<RabbitMQOptions>>().Value));
            services.TryAddSingleton<IRabbitMQConnectionProvider>(provider => provider.GetRequiredService<RabbitMQConnectionProvider>());
            services.TryAddSingleton<IRabbitMQClientFactory, RabbitMQClientFactory>();
            services.TryAddSingleton<IRabbitMQConfigurator>(provider => new RabbitMQConfigurator(
                provider.GetRequiredService<IOptions<RabbitMQOptions>>().Value,
                provider.GetRequiredService<IRabbitMQConnectionProvider>()));

            return services;
        }
    }
}
