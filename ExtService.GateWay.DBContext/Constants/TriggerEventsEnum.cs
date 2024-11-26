namespace ExtService.GateWay.DBContext.Constants
{
    [Flags]
    public enum TriggerEventsEnum
    {
        Insert = 1,
        Update = 2,
        Delete = 4,
        Truncate = 8
    }
}
