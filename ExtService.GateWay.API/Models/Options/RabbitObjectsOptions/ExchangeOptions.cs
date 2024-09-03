using RabbitMQ.Client;

namespace ExtService.GateWay.API.Models.Options.RabbitObjectsOptions
{
    public class ExchangeOptions
    {
        public string Name { get; set; }
        public string Type { get; set; } = ExchangeType.Direct;
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
    }
}
