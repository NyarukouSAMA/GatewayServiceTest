namespace MSP.QueueClient.Models.Options
{
    public class QueueOptions
    {
        public string Name { get; set; }
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}
