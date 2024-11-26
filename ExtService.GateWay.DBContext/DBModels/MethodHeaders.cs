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
        public string Description { get; set; }
        // M-to-1 relationships
        [Required]
        public Guid MethodId { get; set; }
        public MethodInfo Method { get; set; }
        [Required]
        public Guid PluginId { get; set; }
        public Plugins Plugin { get; set; }
        // 1-to-M relationships
        public List<PluginLinks> PluginLinks { get; set; }
    }
}
