using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [Key]
        public Guid UserId { get; set; }
        public string DomainName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int State { get; set; }
        //M-to-1 links
        [Required]
        public Guid SystemId { get; set; }
        public SystemInfo SystemInfo { get; set; }
    }
}
