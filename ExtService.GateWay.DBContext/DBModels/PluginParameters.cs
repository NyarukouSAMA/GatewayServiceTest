using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("PluginParameters")]
    public class PluginParameters
    {
        [Key]
        public Guid ParameterId { get; set; }
        [Required]
        public string ParameterName { get; set; }
        [Required]
        public string ParameterType { get; set; }
        [Required]
        public string ParameterValue { get; set; }
        public string Description { get; set; }
        // 1-to-M relationships
        public List<PluginLinks> PluginLinks { get; set; }
    }
}
