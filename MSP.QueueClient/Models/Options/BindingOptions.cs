namespace MSP.QueueClient.Models.Options
{
    public class BindingOptions
    {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }
}
