using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceRequests;
using System.Text;

namespace ExtService.GateWay.API.Services.SProxing
{
    public class ServiceProxing : IProxingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ServiceProxing> _logger;

        public ServiceProxing(IHttpClientFactory httpClientFactory,
            ILogger<ServiceProxing> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken)
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
                        string errorMessage = $"Неправильное имя метода: {request.RequestMethodName}";
                        _logger.LogError(errorMessage);
                        
                        return new ServiceResponse<string>
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
                    string errorMessage = await httpResponse.GetErrorMessageAsync($@"Во время исполнения запроса по адрессу ""{httpRequest.RequestUri}"" возникла ошибка.");

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
                string headerMessage = "Во время исполнения проксирующего запроса возникла непредвиденная ошибка.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<string>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                };
            }
        }
    }
}
