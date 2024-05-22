namespace ExtService.GateWay.DBContext.DBModels
{
    public class NotificationInfo
    {
        public Guid NotificationId { get; set; }
        public int NotificationLimitPercentage { get; set; }
        public string Message { get; set; }
        //M-to-1 links
        public Guid SystemId { get; set; }
        public SystemInfo SystemInfo { get; set; }
        public Guid BillingId { get; set; }
        public Billing Billing { get; set; }
    }
}
