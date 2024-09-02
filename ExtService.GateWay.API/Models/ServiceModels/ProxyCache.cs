namespace ExtService.GateWay.API.Models.ServiceModels
{
    public class ProxyCache
    {
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string ContentType { get; set; }
        public int StatusCode { get; set; }
    }
}
