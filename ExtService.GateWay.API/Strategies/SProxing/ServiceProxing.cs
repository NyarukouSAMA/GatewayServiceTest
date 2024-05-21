using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using System.Text;

namespace ExtService.GateWay.API.Strategies.SProxing
{
    public class ServiceProxing : IProxingStrategy
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ServiceProxing> _logger;

        public ServiceProxing(IHttpClientFactory httpClientFactory,
            ILogger<ServiceProxing> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<HttpResponseMessage>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient _httpClient;
                switch (request.RequestMethodName)
                {
                    case MethodConstants.SuggestionMethodName:
                        _httpClient = _httpClientFactory.CreateClient(HTTPConstants.SuggestionApiClientName);
                        break;
                    case MethodConstants.CleanerMethodName:
                        _httpClient = _httpClientFactory.CreateClient(HTTPConstants.CleanerApiClientName);
                        break;
                    default:
                        string errorMessage = $"Invalid request method name: {request.RequestMethodName}";
                        _logger.LogError(errorMessage);
                        
                        return new ServiceResponse<HttpResponseMessage>
                        {
                            IsSuccess = false,
                            StatusCode = StatusCodes.Status400BadRequest,
                            ErrorMessage = errorMessage
                        };
                }



                var httpRequest = new HttpRequestMessage
                {
                    Method = request.Method,
                    RequestUri = new Uri(_httpClient.BaseAddress, request.RequestPath)
                };

                if (request.Body != null)
                {
                    httpRequest.Content = new StringContent(request.Body, Encoding.UTF8, "application/json");
                }
                
                var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string errorMessage = await httpResponse.GetErrorMessageAsync($@"Error occurred while processing request to {request.Url}");

                    _logger.LogError(errorMessage);
                    return new ServiceResponse<HttpResponseMessage>()
                    {
                        IsSuccess = false,
                        StatusCode = (int)httpResponse.StatusCode,
                        ErrorMessage = errorMessage
                    };
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                return new ServiceResponse<HttpResponseMessage>()
                {
                    StatusCode = (int)httpResponse.StatusCode,
                    IsSuccess = true,
                    Data = httpResponse
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "An error occurred while processing request.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<HttpResponseMessage>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
