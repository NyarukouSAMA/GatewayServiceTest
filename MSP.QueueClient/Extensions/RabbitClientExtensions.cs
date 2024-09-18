using MSP.QueueClient.Models;
using RabbitMQ.Client.Events;

namespace MSP.QueueClient.Extensions
{
    public static class RabbitClientExtensions
    {
        public static List<XDeath> GetXDeaths(this BasicDeliverEventArgs args)
        {
            var xDeaths = new List<XDeath>();
            if (args.BasicProperties.Headers != null && args.BasicProperties.Headers.ContainsKey("x-death"))
            {
                var xDeathList = args.BasicProperties.Headers["x-death"] as List<object>;
                if (xDeathList != null)
                {
                    foreach (var xDeath in xDeathList)
                    {
                        var xDeathDict = xDeath as Dictionary<string, object>;
                        if (xDeathDict != null)
                        {
                            var xDeathItem = new XDeath
                            {
                                Count = xDeathDict["count"] != null ? Convert.ToInt64(xDeathDict["count"]) : 0,
                                Exchange = xDeathDict["exchange"]?.ToString(),
                                Queue = xDeathDict["queue"]?.ToString(),
                                Reason = xDeathDict["reason"]?.ToString(),
                                Time = xDeathDict["time"] != null ? Convert.ToDateTime(xDeathDict["time"]) : null,
                                RoutingKeys = xDeathDict["routing-keys"] as List<object>
                            };
                            xDeaths.Add(xDeathItem);
                        }
                    }
                }
            }

            return xDeaths;
        }
    }
}
