using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("PluginLinks")]
    public class PluginLinks
    {
        [Key]
        public Guid LinkId { get; set; }
        // M-to-1 relationships
        [Required]
        public Guid MethodHeaderId { get; set; }
        public MethodHeaders MethodHeader { get; set; }
        [Required]
        public Guid PluginId { get; set; }
        public Plugins Plugin { get; set; }
        [Required]
        public Guid ParameterId { get; set; }
        public PluginParameters Parameter { get; set; }
    }
}
