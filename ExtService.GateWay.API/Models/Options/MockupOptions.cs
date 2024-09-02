namespace ExtService.GateWay.API.Models.Options
{
    public class MockupOptions
    {
        public static string MockupOptionsSection = "Mockups";
        public bool BillingMockup { get; set; }
        public bool ClientIdentificationMockup { get; set; }
        public bool ProxyMockup { get; set; }
        public bool MethodInfoMockup { get; set; }
        public bool MockupJWTRegistration { get; set; }
        public bool LimitCheckMockup { get; set; }
    }
}
