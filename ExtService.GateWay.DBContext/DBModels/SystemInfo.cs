using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("SystemInfo")]
    public class SystemInfo
    {
        [Key]
        public Guid SystemId { get; set; }
        [Required]
        public string SystemName { get; set; }
        //1-to-M links
        public ICollection<UserInfo> Users { get; set; }
        public ICollection<Identification> IdentificationSet { get; set; }
        public ICollection<NotificationInfo> NotificationInfoSet { get; set; }
    }
}
