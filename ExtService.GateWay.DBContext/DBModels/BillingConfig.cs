using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("BillingConfig")]
    public class BillingConfig
    {
        [Key]
        public Guid BillingConfigId { get; set; }
        [Required]
        public int PeriodInDays { get; set; }
        [Required]
        public int RequestLimitPerPeriod { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        //M-to-1 relationships
        [Required]
        public Guid IdentificationId { get; set; }
        public Identification Identification { get; set; }
        [Required]
        public Guid MethodId { get; set; }
        public MethodInfo Method { get; set; }
        //1-to-M relationships
        public ICollection<NotificationInfo> NotificationInfoSet { get; set; }
        public ICollection<Billing> BillingRecords { get; set; }
    }
}
