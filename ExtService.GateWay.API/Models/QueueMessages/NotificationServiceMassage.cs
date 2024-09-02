using System.ComponentModel.DataAnnotations;

namespace ExtService.GateWay.API.Models.QueueMessages
{
    public class NotificationServiceMassage
    {
        public string RecipientList { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
