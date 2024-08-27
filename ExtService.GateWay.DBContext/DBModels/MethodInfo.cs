using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("MethodInfo")]
    public class MethodInfo
    {
        [Key]
        public Guid MethodId { get; set; }
        [Required]
        public string MethodName { get; set; }
        [Required]
        public string MethodPath { get; set; }
        [Required]
        public string ApiBaseUri { get; set; }
        public string ApiPrefix { get; set; }
        public int ApiTimeout { get; set; }
        //1-to-M links
        public ICollection<BillingConfig> BillingConfigSet { get; set; }
        public ICollection<Billing> BillingSet { get; set; }
        public ICollection<SubMethodInfo> SubMethodInfoSet { get; set; }
        public ICollection<MethodHeaders> MethodHeaders { get; set; }
    }
}
