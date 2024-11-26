namespace ExtService.GateWay.DBContext.DBFunctions
{
    public abstract class DBAssignableFunction<TInput, TResult> : IDBAssugnableFunction
    {
        public static TResult Execute(TInput input)
        {
            throw new InvalidOperationException("This method is meant to be used with LINQ queries to call the database function.");
        }
    }
}
