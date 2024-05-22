namespace ExtService.GateWay.DBContext.DBModels
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string DomainName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int State { get; set; }
        //M-to-1 links
        public Guid SystemId { get; set; }
        public SystemInfo SystemInfo { get; set; }
    }
}
