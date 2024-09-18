using RabbitMQ.Client;

namespace MSP.QueueClient.Models.Options
{
    public class ExchangeOptions
    {
        public string Name { get; set; }
        public string Type { get; set; } = ExchangeType.Direct;
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
    }
}
