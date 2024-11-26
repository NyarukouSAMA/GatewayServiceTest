namespace ExtService.GateWay.DBContext.Utilities.Models
{
    public class FunctionInfo : IEquatable<FunctionInfo>
    {
        public string Schema { get; set; }
        public string FunctionName { get; set; }
        public string FunctionBody { get; set; }

        public bool Equals(FunctionInfo other)
        {
            return Schema == other.Schema
                && FunctionName == other.FunctionName
                && FunctionBody.Trim() == other.FunctionBody.Trim();
        }

        public override bool Equals(object obj)
        {
            return obj is FunctionInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Schema.GetHashCode() ^ FunctionName.GetHashCode() ^ FunctionBody.GetHashCode();
        }
    }
}
