using ExtService.GateWay.DBContext.Constants;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ExtService.GateWay.DBContext.Utilities.MigrationOperations
{
    public class CreateTriggerOperation : MigrationOperation
    {
        public string TriggerName { get; set; }
        public string TableName { get; set; }
        public TriggerTypeEnum TriggerType { get; set; }
        public string TriggerFunction { get; set; } 
        public TriggerTimingsEnum TriggerTiming { get; set; }
        public TriggerEventsEnum TriggerEvents { get; set; }
        public string[] TriggerColumns { get; set; }
    }
}
