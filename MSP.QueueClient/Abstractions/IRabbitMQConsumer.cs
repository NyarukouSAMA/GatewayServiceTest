namespace MSP.QueueClient.Abstractions
{
    public interface IRabbitMQConsumer
    {
        public void StartConsuming();
        public void StopConsuming();
    }
}
