using System.ComponentModel.DataAnnotations;

namespace ExtService.GateWay.API.Models.ServiceModels
{
    public class LimitCheckResponse
    {
        public Guid NotificationId { get; set; }
        public bool SendNotification { get; set; }
        public int NotificationLimitPercentage { get; set; }
        public int CurrentPercentage { get; set; }
        public string RecipientList { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
