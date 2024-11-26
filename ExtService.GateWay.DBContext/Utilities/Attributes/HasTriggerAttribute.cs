using ExtService.GateWay.DBContext.Constants;

namespace ExtService.GateWay.DBContext.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class HasTriggerAttribute : Attribute
    {
        public string TriggerName { get; set; }
        public TriggerTypeEnum TriggerType { get; set; }
        public string TriggerFunction { get; set; }
        public TriggerTimingsEnum TriggerTiming { get; set; }
        public TriggerEventsEnum TriggerEvents { get; set; }
        public string[] TriggerColumns { get; set; }

        public HasTriggerAttribute(
            string triggerName,
            TriggerTypeEnum triggerType,
            string triggerFunction,
            TriggerTimingsEnum triggerTiming,
            TriggerEventsEnum triggerEvents,
            params string[] triggerColumns)
        {
            TriggerName = triggerName;
            TriggerType = triggerType;
            TriggerFunction = triggerFunction;
            TriggerTiming = triggerTiming;
            TriggerEvents = triggerEvents;
            TriggerColumns = triggerColumns?.Length > 0 ? triggerColumns : new string[0];
        }
    }
}
