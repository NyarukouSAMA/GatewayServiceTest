namespace MSP.QueueClient.Abstractions
{
    public interface IRabbitMQClientFactory
    {
        public IRabbitMQConsumer GetConsumer<TData>();
        public IRabbitMQProducer GetProducer();
        public IRabbitMQConfigurator GetConfigurator();
    }
}
