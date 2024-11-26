using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ExtService.GateWay.DBContext.Utilities.MigrationOperations
{
    public class DropTriggerOperation : MigrationOperation
    {
        public string TriggerName { get; set; }
        public string TableName { get; set; }
    }
}
