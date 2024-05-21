namespace ExtService.GateWay.API.Models.Options
{
    public class ProxyOptions
    {
        public static string ProxyOptionsSection = "Proxy";
        public string SuggestionApiBaseUrl { get; set; }
        public string SuggestionApiPrefix { get; set; }
        public string SuggestionApiToken { get; set; }
        public int SuggestionApiTimeoutInSeconds { get; set; }
        public string CleanerApiBaseUrl { get; set; }
        public string CleanerApiPrefix { get; set; }
        public string CleanerApiToken { get; set; }
        public int CleanerApiTimeoutInSeconds { get; set; }
    }
}
