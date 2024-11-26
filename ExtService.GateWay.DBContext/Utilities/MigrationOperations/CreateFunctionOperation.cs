using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ExtService.GateWay.DBContext.Utilities.MigrationOperations
{
    public class CreateFunctionOperation : MigrationOperation
    {
        public string Schema { get; set; }
        public string FunctionName { get; set; }
        public string FunctionBody { get; set; }
    }
}
