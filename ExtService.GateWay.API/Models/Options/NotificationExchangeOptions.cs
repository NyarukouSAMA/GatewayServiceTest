namespace ExtService.GateWay.API.Models.Options
{
    public class NotificationExchangeOptions
    {
        public static string NotificationExchangeOptionsSection = "NotificationExchange";
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }
}
