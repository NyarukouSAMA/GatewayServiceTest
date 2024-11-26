using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ExtService.GateWay.DBContext.Utilities.MigrationOperations
{
    public class DropFunctionOperation : MigrationOperation
    {
        public string Schema { get; set; }
        public string FunctionName { get; set; }
    }
}
