using ExtService.GateWay.DBContext.Utilities.Models;

namespace ExtService.GateWay.DBContext.Utilities.Comparers
{
    public class FunctionComparer : IEqualityComparer<FunctionInfo>
    {
        public bool Equals(FunctionInfo? x, FunctionInfo? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public int GetHashCode(FunctionInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}
