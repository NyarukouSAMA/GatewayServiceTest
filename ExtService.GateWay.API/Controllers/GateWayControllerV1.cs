using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.HandlerResponses;
using ExtService.GateWay.API.Models.Logging;
using ExtService.GateWay.API.Models.Requests;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtService.GateWay.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GateWayControllerV1 : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _requestLogger;

        public GateWayControllerV1(IMediator mediator,
            ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _requestLogger = loggerFactory.CreateLogger(LoggingConstants.RequestLogCategory);
        }

        [HttpPost("proxy")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Proxy([FromBody] PostProxyRequest requestContent)
        {
            var clientId = User.Claims.FirstOrDefault(c => c.Type == "azp")?.Value;
            if (string.IsNullOrEmpty(clientId))
            {
                string errorMessage = "Client id not found in token.";
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = errorMessage
                }));
                return Unauthorized(errorMessage);
            }

            if (requestContent == null
                || requestContent.RequestBody is JObject && ((JObject)requestContent.RequestBody).Count == 0
                || requestContent.RequestBody is JArray && ((JArray)requestContent.RequestBody).Count == 0
                || requestContent.RequestBody is string && string.IsNullOrEmpty((string)requestContent.RequestBody))
            {
                string errorMessage = "Request body is empty.";
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = errorMessage
                }));
                return BadRequest(errorMessage);
            }

            string requestBody = requestContent.RequestBody.ToString();

            var identificationResponce = await _mediator.Send(new IdentificationHandlerModel
            {
                ClientId = clientId,
                MethodName = requestContent.MethodName,
                SubMethodName = requestContent.SubMethodName,
            });

            if (!identificationResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = identificationResponce.ErrorMessage,
                    IsSuccess = identificationResponce.IsSuccess,
                    StatusCode = identificationResponce.StatusCode,
                    RequestData = requestBody
                }));

                return StatusCode(identificationResponce.StatusCode, identificationResponce.ErrorMessage);
            }

            
            bool ignoreCache = Request.Headers.TryGetValue("IgnoreCache", out var ignoreCacheValue)
                && bool.TryParse(ignoreCacheValue, out var ignoreCacheParseResult)
                && ignoreCacheParseResult;

            if (!ignoreCache)
            {
                var getProxyCacheResult = await _mediator.Send(new GetProxyCacheHandlerModel
                {
                    KeyInput = requestContent,
                    KeyPrefix = MethodConstants.SuggestionMethodName
                });

                if (!getProxyCacheResult.IsSuccess && getProxyCacheResult.StatusCode != StatusCodes.Status404NotFound)
                {
                    _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                    {
                        ErrorMessage = getProxyCacheResult.ErrorMessage,
                        IsSuccess = getProxyCacheResult.IsSuccess,
                        StatusCode = getProxyCacheResult.StatusCode,
                        RequestData = requestContent
                    }));

                    return StatusCode(getProxyCacheResult.StatusCode, getProxyCacheResult.ErrorMessage);
                }

                if (getProxyCacheResult.IsSuccess)
                {
                    _requestLogger.LogInformation(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                    {
                        ErrorMessage = getProxyCacheResult.ErrorMessage,
                        IsSuccess = getProxyCacheResult.IsSuccess,
                        StatusCode = getProxyCacheResult.StatusCode,
                        CacheHit = true,
                        RequestData = requestContent,
                        ResponseData = getProxyCacheResult.Data.ProxyResponse
                    }));

                    return Ok(getProxyCacheResult.Data.ProxyResponse);
                }
            }
            
            var billingResponce = await _mediator.Send(new BillingHandlerModel
            {
                ClientId = clientId,
                MethodId = identificationResponce.Data.MethodId,
                IdentificationId = identificationResponce.Data.IdentificationId,
                CurrentDate = DateTime.UtcNow
            });

            if (!billingResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = billingResponce.ErrorMessage,
                    IsSuccess = billingResponce.IsSuccess,
                    StatusCode = billingResponce.StatusCode,
                    RequestData = requestBody
                }));

                return StatusCode(billingResponce.StatusCode, billingResponce.ErrorMessage);
            }

            var request = new ProxyRequest
            {
                Method = HttpMethod.Post,
                RequestUri = identificationResponce.Data.RequestUri,
                MethodHeaders = identificationResponce.Data.MethodHeaders,
                ApiTimeout = identificationResponce.Data.ApiTimeout,
                Body = requestBody
            };

            var proxyResponce = await _mediator.Send(request);
            if (!proxyResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = proxyResponce.ErrorMessage,
                    IsSuccess = proxyResponce.IsSuccess,
                    StatusCode = proxyResponce.StatusCode,
                    RequestData = requestBody
                }));
                return StatusCode(proxyResponce.StatusCode, proxyResponce.ErrorMessage);
            }

            var stringContent = await proxyResponce.Data.ReadAsStringAsync();

            _requestLogger.LogInformation(JsonConvert.SerializeObject(new LoggingRequest<string, string>
            {
                ErrorMessage = proxyResponce.ErrorMessage,
                IsSuccess = proxyResponce.IsSuccess,
                StatusCode = proxyResponce.StatusCode,
                RequestData = requestBody,
                ResponseData = stringContent
            }));

            return new ContentResult
            {
                Content = stringContent,
                ContentType = proxyResponce.Data.Headers.ContentType?.ToString(),
                StatusCode = proxyResponce.StatusCode
            };
        }
    }
}
