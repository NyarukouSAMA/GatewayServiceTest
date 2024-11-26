using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("Plugins")]
    public class Plugins
    {
        [Key]
        public Guid PluginId {  get; set; }
        [Required]
        public string PluginName { get; set; }
        // 1-to-M relationships
        public List<MethodHeaders> MethodHeaders { get; set; }
        public List<PluginLinks> PluginLinks { get; set; }
    }
}
