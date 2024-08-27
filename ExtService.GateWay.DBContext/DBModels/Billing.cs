using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("Billing")]
    public class Billing
    {
        [Key]
        public Guid BillingId { get; set; }
        [Required]
        public int RequestLimit { get; set; }
        [Required]
        public int RequestCount { get; set; }
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
        [Required]
        public Guid BillingConfigId { get; set; }
        public BillingConfig BillingConfig { get; set; }
    }
}
