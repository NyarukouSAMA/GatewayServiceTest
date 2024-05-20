namespace ExtService.GateWay.API.Models.Logging
{
    public class LoggingRequest<TRequest, TResponse>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public TRequest RequestData { get; set; }
        public TResponse ResponseData { get; set; }
    }
}
