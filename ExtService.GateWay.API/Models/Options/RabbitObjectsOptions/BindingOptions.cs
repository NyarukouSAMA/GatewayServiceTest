namespace ExtService.GateWay.API.Models.Options.RabbitObjectsOptions
{
    public class BindingOptions
    {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }
}
