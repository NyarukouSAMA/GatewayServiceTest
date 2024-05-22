namespace ExtService.GateWay.DBContext.DBModels
{
    public class SystemInfo
    {
        public Guid SystemId { get; set; }
        public string SystemName { get; set; }
        //1-to-M links
        public ICollection<UserInfo> Users { get; set; }
        public ICollection<Identification> IdentificationSet { get; set; }
        public ICollection<NotificationInfo> NotificationInfoSet { get; set; }
    }
}
