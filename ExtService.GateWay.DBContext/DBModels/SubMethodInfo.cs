using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //M-to-1 relationships
        [Required]
        public Guid MethodId { get; set; }
        public MethodInfo Method { get; set; }
    }
}
