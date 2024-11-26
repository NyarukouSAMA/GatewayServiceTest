using ExtService.GateWay.DBContext.Constants;
using ExtService.GateWay.DBContext.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtService.GateWay.DBContext.DBModels
{
    [Table("Identification")]
    [HasTrigger(
        triggerName: "trg_identification_logging",
        triggerType: TriggerTypeEnum.Row,
        triggerFunction: "log_transaction",
        triggerTiming: TriggerTimingsEnum.After,
        triggerEvents: TriggerEventsEnum.Insert | TriggerEventsEnum.Update | TriggerEventsEnum.Delete
    )]
    public class Identification
    {
        [Key]
        public Guid IdentificationId { get; set; }
        [Required]
        public string ClientId { get; set; }
        public string EnvName { get; set; }
        //M-to-1 relationships
        [Required]
        public Guid SystemId { get; set; }
        public SystemInfo SystemInfo { get; set; }
        //1-to-M relationships
        public ICollection<BillingConfig> BillingConfigSet { get; set; }
        public ICollection<Billing> BillingSet { get; set; }
    }
}
