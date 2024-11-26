using ExtService.GateWay.DBContext.Constants;

namespace ExtService.GateWay.DBContext.DBFunctions.PGSQLAssignableFunctions
{
    public class DecryptPluginParameterValueFunction
    {
        public const string Schema = PGSQLConsts.PublicSchema;
        public const string FunctionName = PGSQLFunction.DecryptPluginParameterValueFunctionName;
        public static string Execute(string input)
        {
            throw new InvalidOperationException("This method is meant to be used with LINQ queries to call the database function.");
        }
    }
}
