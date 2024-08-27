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
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceProxing> _logger;

        public ServiceProxing(IHttpClientFactory httpClientFactory,
            ILogger<ServiceProxing> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<ServiceResponse<HttpContent>> ExecuteAsync(ProxyRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.MethodHeaders != null)
                {
                    foreach (var header in request.MethodHeaders)
                    {
                        _httpClient.DefaultRequestHeaders.Add(header.HeaderName, header.HeaderValue);
                    }
                }

                if (request.ApiTimeout > 0)
                {
                    _httpClient.Timeout = TimeSpan.FromSeconds(request.ApiTimeout);
                }

                var httpRequest = new HttpRequestMessage
                {
                    Method = request.Method,
                    RequestUri = new Uri(request.RequestUri)
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
                    return new ServiceResponse<HttpContent>()
                    {
                        IsSuccess = false,
                        StatusCode = (int)httpResponse.StatusCode,
                        ErrorMessage = errorMessage
                    };
                }

                return new ServiceResponse<HttpContent>()
                {
                    StatusCode = (int)httpResponse.StatusCode,
                    IsSuccess = true,
                    Data = httpResponse.Content
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время исполнения проксирующего запроса возникла непредвиденная ошибка.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<HttpContent>()
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = headerMessage
                };
            }
        }
    }
}
