using ExtService.GateWay.DBContext.Constants;
using ExtService.GateWay.DBContext.Utilities.MigrationOperations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace ExtService.GateWay.DBContext.Utilities
{
    public class GateWayMigrationsSqlGenerator : NpgsqlMigrationsSqlGenerator
    {
        public GateWayMigrationsSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            INpgsqlSingletonOptions npgsqlSingletonOptions)
            : base(dependencies, npgsqlSingletonOptions)
        {
        }

        protected override void Generate(MigrationOperation operation, IModel? model, MigrationCommandListBuilder builder)
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
                base.Generate(operation, model, builder);
            }
        }

        private void GenerateDropTrigger(DropTriggerOperation operation, MigrationCommandListBuilder builder)
        {
            builder.AppendLine($@"DROP TRIGGER IF EXISTS {operation.TriggerName} ON ""{operation.TableName}"";");
        }

        private void GenerateDropFunction(DropFunctionOperation operation, MigrationCommandListBuilder builder)
        {
            builder.AppendLine($@"DROP FUNCTION IF EXISTS ""{operation.Schema}"".""{operation.FunctionName}"";");
        }

        private void GenerateCreateTrigger(CreateTriggerOperation operation, MigrationCommandListBuilder builder)
        {
            string createTriggerSql;

            string triggerTiming = operation.TriggerTiming.ToString().ToUpper();
            string triggerEvents = GetTriggerEvents(operation.TriggerEvents);
            string triggerType = operation.TriggerType.ToString().ToUpper();


            if (operation.TriggerColumns != null && operation.TriggerColumns.Length > 0)
            {
                string columns = string.Join(", ", operation.TriggerColumns.Select(c => $@"""{c}"""));

                createTriggerSql = $@"
                    CREATE TRIGGER {operation.TriggerName}
                    {triggerTiming} {triggerEvents} OF {columns} ON ""{operation.TableName}""
                    FOR EACH {triggerType} EXECUTE FUNCTION {operation.TriggerFunction}();";
            }
            else
            {
                createTriggerSql = $@"
                    CREATE TRIGGER {operation.TriggerName}
                    {triggerTiming} {triggerEvents} ON ""{operation.TableName}""
                    FOR EACH {triggerType} EXECUTE FUNCTION {operation.TriggerFunction}();";
            }

            builder.AppendLine(createTriggerSql);
        }

        private string GetTriggerEvents(TriggerEventsEnum triggerEvents)
        {
            var events = Enum.GetValues<TriggerEventsEnum>()
                .Where(e => triggerEvents.HasFlag(e))
                .Select(e => e.ToString().ToUpper());

            return string.Join(" OR ", events);
        }

        private void GenerateCreateFunction(CreateFunctionOperation operation, MigrationCommandListBuilder builder)
        {
            builder.AppendLine(operation.FunctionBody);
        }
    }
}
