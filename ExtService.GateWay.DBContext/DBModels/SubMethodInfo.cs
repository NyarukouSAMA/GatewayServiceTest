using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("SubMethodInfo")]
    public class SubMethodInfo
    {
        [Key]
        public Guid SubMethodId { get; set; }
        [Required]
        public string SubMethodName { get; set; }
        [Required]
        public string SubMethodPath { get; set; }
        [Required]
        public string HttpMethodName { get; set; }
        //M-to-1 relationships
        [Required]
        public Guid MethodId { get; set; }
        public MethodInfo Method { get; set; }
    }
}
