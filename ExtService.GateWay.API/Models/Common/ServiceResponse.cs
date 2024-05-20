namespace ExtService.GateWay.API.Models.Common
{
    public class ServiceResponse<TResponseData>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public TResponseData Data { get; set; }
    }
}
