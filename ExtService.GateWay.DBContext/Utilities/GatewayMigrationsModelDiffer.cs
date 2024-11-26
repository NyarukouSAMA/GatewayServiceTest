using ExtService.GateWay.DBContext.DBFunctions;
using ExtService.GateWay.DBContext.Utilities.Attributes;
using ExtService.GateWay.DBContext.Utilities.Comparers;
using ExtService.GateWay.DBContext.Utilities.MigrationOperations;
using ExtService.GateWay.DBContext.Utilities.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Reflection;

namespace ExtService.GateWay.DBContext.Utilities
{
#pragma warning disable EF1001 // Internal EF Core API usage.
    public class GatewayMigrationsModelDiffer : MigrationsModelDiffer
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        public GatewayMigrationsModelDiffer(
            IRelationalTypeMappingSource typeMappingSource,
            IMigrationsAnnotationProvider migrationsAnnotationProvider,
            IRowIdentityMapFactory rowIdentityMapFactory, 
            CommandBatchPreparerDependencies commandBatchPreparerDependencies)
            : base(typeMappingSource, migrationsAnnotationProvider, rowIdentityMapFactory, commandBatchPreparerDependencies)
        {
        }

        protected override IEnumerable<MigrationOperation> Diff(IRelationalModel? source,
            IRelationalModel? target,
            DiffContext diffContext)
        {
            var operations = base.Diff(source, target, diffContext).ToList();

            //operations.AddRange(DiffIDBFunctions(source, target));
            operations.AddRange(DiffTrigger(source, target));

            return operations;
        }

        //private IEnumerable<MigrationOperation> DiffIDBFunctions(IRelationalModel? source,
        //    IRelationalModel? target)
        //{
        //    var functionOperations = new List<MigrationOperation>();

        //    var sourceFunctions = GetIDBFunctionsInfo(source);
        //    var targetFunctions = GetIDBFunctionsInfo(target);

        //    // Functions to delete
        //    foreach (var sourceFunction in sourceFunctions.Except(targetFunctions, new FunctionComparer()))
        //    {
        //        functionOperations.Add(new DropFunctionOperation
        //        {
        //            Schema = sourceFunction.Schema,
        //            FunctionName = sourceFunction.FunctionName
        //        });
        //    }

        //    // Functions to add
        //    foreach (var targetFunction in targetFunctions.Except(sourceFunctions, new FunctionComparer()))
        //    {
        //        functionOperations.Add(new CreateFunctionOperation
        //        {
        //            Schema = targetFunction.Schema,
        //            FunctionName = targetFunction.FunctionName,
        //            FunctionBody = targetFunction.FunctionBody
        //        });
        //    }

        //    return functionOperations;
        //}

        //private List<FunctionInfo> GetIDBFunctionsInfo(IRelationalModel? model)
        //{
        //    if (model == null)
        //    {
        //        return new List<FunctionInfo>();
        //    }

        //    var functionTypes = Assembly.GetExecutingAssembly()
        //        .GetTypes()
        //        .Where(t => typeof(IDBAssugnableFunction).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        //    return functionTypes.Select(Activator.CreateInstance).Cast<IDBAssugnableFunction>().Select(f => new FunctionInfo
        //    {
        //        Schema = f.Schema,
        //        FunctionName = f.FunctionName,
        //        FunctionBody = f.FunctionBody
        //    }).ToList();
        //}

        private IEnumerable<MigrationOperation> DiffTrigger(IRelationalModel? source,
            IRelationalModel? target)
        {
            var triggerOperations = new List<MigrationOperation>();

            var sourceTriggers = GetTriggers(source);
            var targetTriggers = GetTriggers(target);

            // Triggers to delete
            foreach (var sourceTrigger in sourceTriggers.Except(targetTriggers, new TriggerComparer()))
            {
                triggerOperations.Add(new DropTriggerOperation
                {
                    TriggerName = sourceTrigger.TriggerName,
                    TableName = sourceTrigger.TriggerTableName
                });
            }

            // Triggers to add
            foreach (var targetTrigger in targetTriggers.Except(sourceTriggers, new TriggerComparer()))
            {
                triggerOperations.Add(new CreateTriggerOperation
                {
                    TriggerName = targetTrigger.TriggerName,
                    TableName = targetTrigger.TriggerTableName,
                    TriggerType = targetTrigger.TriggerType,
                    TriggerFunction = targetTrigger.TriggerFunction,
                    TriggerTiming = targetTrigger.TriggerTiming,
                    TriggerEvents = targetTrigger.TriggerEvents,
                    TriggerColumns = targetTrigger.TriggerColumns
                });
            }

            return triggerOperations;
        }

        private List<HasTriggerAttributeInfo> GetTriggers(IRelationalModel? model)
        {
            var triggers = new List<HasTriggerAttributeInfo>();

            if (model == null)
            {
                return triggers;
            }

            foreach (var table in model.Tables)
            {
                foreach (var entityType in table.EntityTypeMappings.Select(e => e.EntityType))
                {
                    var triggerAttributes = entityType.ClrType?.GetCustomAttributes<HasTriggerAttribute>(true).ToList();

                    if (triggerAttributes == null || triggerAttributes.Count == 0)
                    {
                        continue;
                    }

                    var tableName = table.Name;

                    foreach (var triggerAttribute in triggerAttributes)
                    {
                        triggers.Add(new HasTriggerAttributeInfo
                        {
                            TriggerName = triggerAttribute.TriggerName,
                            TriggerTableName = tableName,
                            TriggerType = triggerAttribute.TriggerType,
                            TriggerFunction = triggerAttribute.TriggerFunction,
                            TriggerTiming = triggerAttribute.TriggerTiming,
                            TriggerEvents = triggerAttribute.TriggerEvents,
                            TriggerColumns = triggerAttribute.TriggerColumns
                        });
                    }
                }
            }

            return triggers;
        }
    }
}
