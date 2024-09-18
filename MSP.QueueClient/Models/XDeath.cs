namespace MSP.QueueClient.Models
{
    public class XDeath
    {
        /// <summary>
        /// Number of messages in the queue.
        /// </summary>
        public long? Count { get; set; }
        /// <summary>
        /// Exchange name.
        /// </summary>
        public string Exchange { get; set; }
        /// <summary>
        /// Queue name.
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// Reason why the message was dead-lettered.
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// Key list of message routing.
        /// </summary>
        public List<object> RoutingKeys { get; set; }
        /// <summary>
        /// Message time.
        /// </summary>
        public DateTime? Time { get; set; }
    }
}
