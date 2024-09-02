using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("NotificationInfo")]
    public class NotificationInfo
    {
        [Key]
        public Guid NotificationId { get; set; }
        [Required]
        public int NotificationLimitPercentage { get; set; }
        [Required]
        public string RecipientList { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        //M-to-1 links
        public Guid BillingId { get; set; }
        public Billing Billing { get; set; }
        [Required]
        public Guid BillingConfigId { get; set; }
        public BillingConfig BillingConfig { get; set; }
    }
}
