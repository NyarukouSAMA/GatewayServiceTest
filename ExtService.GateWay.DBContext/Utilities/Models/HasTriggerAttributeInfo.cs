using ExtService.GateWay.DBContext.Constants;

namespace ExtService.GateWay.DBContext.Utilities.Models
{
    public class HasTriggerAttributeInfo : IEquatable<HasTriggerAttributeInfo>
    {
        public string TriggerName { get; set; }
        public string TriggerTableName { get; set; }
        public TriggerTypeEnum TriggerType { get; set; }
        public string TriggerFunction { get; set; }
        public TriggerTimingsEnum TriggerTiming { get; set; }
        public TriggerEventsEnum TriggerEvents { get; set; }
        public string[] TriggerColumns { get; set; }

        public bool Equals(HasTriggerAttributeInfo? other)
        {
            if (other == null)
            {
                return false;
            }

            return TriggerName == other.TriggerName
                && TriggerTableName == other.TriggerTableName
                && TriggerType == other.TriggerType
                && TriggerFunction == other.TriggerFunction
                && TriggerTiming == other.TriggerTiming
                && TriggerEvents == other.TriggerEvents
                && ColumnsEqual(TriggerColumns, other.TriggerColumns);
        }

        public override bool Equals(object? obj)
        {
            if (obj is HasTriggerAttributeInfo other)
            {
                return Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int columnsHashCode = TriggerColumns?.Any() == true
                ? TriggerColumns.Select(c => c.GetHashCode()).Aggregate((a, b) => a ^ b)
                : 0;

            return TriggerName.GetHashCode()
                ^ TriggerTableName.GetHashCode()
                ^ TriggerType.GetHashCode()
                ^ TriggerFunction.GetHashCode()
                ^ TriggerTiming.GetHashCode()
                ^ TriggerEvents.GetHashCode()
                ^ columnsHashCode;
        }

        private bool ColumnsEqual(string[]? columns1, string[]? columns2)
        {
            if (columns1 == null && columns2 == null)
            {
                return true;
            }

            if (columns1 == null || columns2 == null)
            {
                return false;
            }

            if (columns1.Length != columns2.Length)
            {
                return false;
            }

            return columns1.OrderBy(c => c).SequenceEqual(columns2.OrderBy(c => c));
        }
    }
}
