using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.DBContext.DBModels;
using System.Text;

namespace ExtService.GateWay.API.Services.SProxing
{
    public class ServiceProxing : IProxingService
    {
        private readonly HttpClient _httpClient;
        private readonly IRestProxyContentTransformerFactory _restProxyContentTransformerFactory;
        private readonly IPluginFactory _pluginFactory;
        private readonly ILogger<ServiceProxing> _logger;

        public ServiceProxing(IHttpClientFactory httpClientFactory,
            IRestProxyContentTransformerFactory restProxyContentTransformerFactory,
            IPluginFactory pluginFactory,
            ILogger<ServiceProxing> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _restProxyContentTransformerFactory = restProxyContentTransformerFactory;
            _pluginFactory = pluginFactory;
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
                        IPlugin headerHandler = _pluginFactory.GetPlugin(header.Plugin.PluginName);

                        Dictionary<string, PluginParameters> pluginParameters = header.PluginLinks
                            .Select(link => link.Parameter)
                            .ToDictionary(pp => pp.ParameterName);

                        var headerResult = await headerHandler.ExecuteAsync(
                            header,
                            pluginParameters,
                            cancellationToken);
                        if (!headerResult.IsSuccess)
                        {
                            string errorMessage = $"Во время исполнения плагина {header.Plugin.PluginName} возникла ошибка.";

                            _logger.LogError(errorMessage);
                            return new ServiceResponse<HttpContent>()
                            {
                                IsSuccess = false,
                                StatusCode = headerResult.StatusCode,
                                ErrorMessage = errorMessage
                            };
                        }

                        _httpClient.DefaultRequestHeaders.Add(header.HeaderName, headerResult.Data);
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
                    var contentTransformer = _restProxyContentTransformerFactory.GetRestProxyContentTransformer(request.Method.Method);
                    httpRequest = await contentTransformer.TransformAsync(httpRequest, request.Body, cancellationToken);
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
