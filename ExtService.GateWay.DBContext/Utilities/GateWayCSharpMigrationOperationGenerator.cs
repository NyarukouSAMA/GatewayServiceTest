using ExtService.GateWay.DBContext.Constants;
using ExtService.GateWay.DBContext.Utilities.MigrationOperations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ExtService.GateWay.DBContext.Utilities
{
    public class GateWayCSharpMigrationOperationGenerator : CSharpMigrationOperationGenerator
    {
        public GateWayCSharpMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        protected override void Generate(MigrationOperation operation, IndentedStringBuilder builder)
        {
            if (operation is DropTriggerOperation dropTriggerOperation)
            {
                GenerateDropTrigger(dropTriggerOperation, builder);
            }
            else if (operation is DropFunctionOperation dropFunctionOperation)
            {
                GenerateDropFunction(dropFunctionOperation, builder);
            }
            else if (operation is CreateTriggerOperation createTriggerOperation)
            {
                GenerateCreateTrigger(createTriggerOperation, builder);
            }
            else if (operation is CreateFunctionOperation createFunctionOperation)
            {
                GenerateCreateFunction(createFunctionOperation, builder);
            }
            else
            {
                base.Generate(operation, builder);
            }
        }

        private void GenerateDropTrigger(DropTriggerOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine($@"migrationBuilder.Sql(""DROP TRIGGER IF EXISTS {operation.TriggerName} ON ""{operation.TableName}"";"");");
        }

        private void GenerateDropFunction(DropFunctionOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine($@"migrationBuilder.Sql(""DROP FUNCTION IF EXISTS ""{operation.Schema}"".""{operation.FunctionName}"";"");");
        }

        private void GenerateCreateTrigger(CreateTriggerOperation operation, IndentedStringBuilder builder)
        {
            string triggerTiming = operation.TriggerTiming.ToString().ToUpper();
            string triggerEvents = GetTriggerEvents(operation.TriggerEvents);
            string triggerType = operation.TriggerType.ToString().ToUpper();

            if (operation.TriggerColumns != null && operation.TriggerColumns.Length > 0)
            {
                string columns = string.Join(", ", operation.TriggerColumns.Select(c => $@"""{c}"""));

                builder.AppendLine($@"migrationBuilder.Sql(@""CREATE TRIGGER {operation.TriggerName}
{triggerTiming} {triggerEvents} OF {columns} ON ""{operation.TableName}""
FOR EACH {triggerType} EXECUTE FUNCTION {operation.TriggerFunction}();"");");
            }
            else
            {
                builder.AppendLine($@"migrationBuilder.Sql(@""CREATE TRIGGER {operation.TriggerName}
{triggerTiming} {triggerEvents} ON ""{operation.TableName}""
FOR EACH {triggerType} EXECUTE FUNCTION {operation.TriggerFunction}();"");");
            }
        }

        private string GetTriggerEvents(TriggerEventsEnum triggerEvents)
        {
            var events = Enum.GetValues<TriggerEventsEnum>()
                .Where(e => triggerEvents.HasFlag(e))
                .Select(e => e.ToString().ToUpper());

            return string.Join(" OR ", events);
        }

        private void GenerateCreateFunction(CreateFunctionOperation operation, IndentedStringBuilder builder)
        {
            builder.AppendLine($@"migrationBuilder.Sql(@""{operation.FunctionBody}"");");
        }
    }
}
