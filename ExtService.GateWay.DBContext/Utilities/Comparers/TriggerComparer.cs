using ExtService.GateWay.DBContext.Utilities.Models;

namespace ExtService.GateWay.DBContext.Utilities.Comparers
{
    public class TriggerComparer : IEqualityComparer<HasTriggerAttributeInfo>
    {
        public bool Equals(HasTriggerAttributeInfo? x, HasTriggerAttributeInfo? y)
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

        public int GetHashCode(HasTriggerAttributeInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}
