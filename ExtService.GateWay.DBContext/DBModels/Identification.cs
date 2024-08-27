using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("Identification")]
    public class Identification
    {
        [Key]
        public Guid IdentificationId { get; set; }
        [Required]
        public string ClientId { get; set; }
        public string EnvName { get; set; }
        //M-to-1 relationships
        [Required]
        public Guid SystemId { get; set; }
        public SystemInfo SystemInfo { get; set; }
        //1-to-M relationships
        public ICollection<BillingConfig> BillingConfigSet { get; set; }
        public ICollection<Billing> BillingSet { get; set; }
    }
}
