using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using System.Text;

namespace ExtService.GateWay.API.Strategies.SProxing
{
    public class ServiceProxing : IProxingStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceProxing> _logger;

        public ServiceProxing(HttpClient httpClient,
            ILogger<ServiceProxing> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), request.Url)
                {
                    Content = new StringContent(request.Body, Encoding.UTF8, "application/json")
                };

                var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string errorMessage = await httpResponse.GetErrorMessageAsync($@"Error occurred while processing request to {request.Url}");

                    _logger.LogError(errorMessage);
                    return new ServiceResponse<string>()
                    {
                        IsSuccess = false,
                        StatusCode = (int)httpResponse.StatusCode,
                        ErrorMessage = errorMessage
                    };
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                return new ServiceResponse<string>()
                {
                    StatusCode = (int)httpResponse.StatusCode,
                    IsSuccess = true,
                    Data = responseContent
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "An error occurred while processing request.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<string>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
