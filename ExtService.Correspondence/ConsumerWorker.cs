using ExtService.Correspondence.Models.HandlerModels;
using ExtService.Correspondence.Models.Options;
using Microsoft.Extensions.Options;
using MSP.QueueClient.Abstractions;

namespace ExtService.Correspondence
{
    /// <summary>
    /// This worker starts all registred consumers and works until the application is stopped.
    /// </summary>
    public class ConsumerWorker : BackgroundService
    {
        private readonly IRabbitMQClientFactory _rabbitMQClientFactory;
        private readonly ServiceQueues _serviceQueueOptions;
        private readonly ILogger<ConsumerWorker> _logger;

        private List<IRabbitMQConsumer> _workingConsumers;

        public ConsumerWorker(IRabbitMQClientFactory rabbitMQClientFactory,
            IOptions<ServiceQueues> serviceQueueOptions,
            ILogger<ConsumerWorker> logger)
        {
            _rabbitMQClientFactory = rabbitMQClientFactory;
            _serviceQueueOptions = serviceQueueOptions.Value;
            _logger = logger;
        }

        // StartAsync method starts all consumers got from IRabbitMQClientFactory
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _rabbitMQClientFactory.GetConfigurator().Configure();

            _logger.LogInformation("ConsumerWorker starting");

            #region Start consumers
            if(_workingConsumers == null)
            {
                _workingConsumers = new List<IRabbitMQConsumer>();
            }

            var emailConsumer = _rabbitMQClientFactory.GetConsumer<EmailQueueMessage>();
            emailConsumer.StartConsuming();
            _workingConsumers.Add(emailConsumer);


            #endregion

            await base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Worker will work until the application is stopped
            return Task.CompletedTask;
        }

        // StopAsync method stops all consumers got from IRabbitMQClientFactory
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ConsumerWorker stopping");
            
            foreach (var consumer in _workingConsumers)
            {
                consumer.StartConsuming();
            }

            await base.StopAsync(cancellationToken);
        }

        
    }
}