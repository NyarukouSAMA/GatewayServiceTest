using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("MethodHeaders")]
    public class MethodHeaders
    {
        [Key]
        public Guid MethodHeaderId { get; set; }
        [Required]
        public string HeaderName { get; set; }
        [Required]
        public string HeaderValue { get; set; }
        public string Description { get; set; }
        // M-to-1 relationships
        [Required]
        public Guid MethodId { get; set; }
        public MethodInfo Method { get; set; }
    }
}
