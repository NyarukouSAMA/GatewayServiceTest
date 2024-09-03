using ExtService.GateWay.API.Models.Options.RabbitObjectsOptions;

namespace ExtService.GateWay.API.Models.Options
{
    public class RabbitMQOptions
    {
        public static string RabbitMQOptionsSection = "RabbitMQ";
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public List<ExchangeOptions> Exchanges { get; set; } = new List<ExchangeOptions>();
        public List<QueueOptions> Queues { get; set; } = new List<QueueOptions>();
        public List<BindingOptions> Bindings { get; set; } = new List<BindingOptions>();
    }
}
