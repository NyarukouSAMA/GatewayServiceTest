using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("TransactionLog")]
    public class TransactionLog
    {
        [Key]
        public Guid TransactionLogId { get; set; }
        [Required]
        public string TableName { get; set; }
        [Required]
        public string Operation { get; set; }
        [Required]
        public string OldValue { get; set; }
        [Required]
        public string NewValue { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public string DBUser { get; set; }
    }
}
