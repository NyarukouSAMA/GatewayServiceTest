namespace MSP.QueueClient.Models
{
    public class Message<TData>
    {
        /// <summary>
        /// Message headers
        /// </summary>
        public IDictionary<string, object> Headers { get; set; }
        /// <summary>
        /// Message data
        /// </summary>
        public TData Data { get; set; }
        /// <summary>
        /// Routing key
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        /// Correlation id
        /// </summary>
        public string CorrelationId { get; set; }
        /// <summary>
        /// Message id
        /// </summary>
        public string MessageId { get; set; }
        public List<XDeath> XDeath { get; internal set; }
    }
}
