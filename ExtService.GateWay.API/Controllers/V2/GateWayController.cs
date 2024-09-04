using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.Logging;
using ExtService.GateWay.API.Models.ServiceModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ExtService.GateWay.API.Models.Requests.V2;
using Asp.Versioning;

namespace ExtService.GateWay.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class GateWayController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _requestLogger;

        public GateWayController(IMediator mediator,
            ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _requestLogger = loggerFactory.CreateLogger(LoggingConstants.RequestLogCategory);
        }

        [HttpPost("proxy")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Proxy([FromBody] PostProxyRequest requestContent)
        {
            #region Check client id
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
            #endregion

            #region Check request body
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
            #endregion

            string requestBody = requestContent.RequestBody.ToString();

            #region Get method info and identification record
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
            #endregion

            #region Get value from Cache (if flag's value false)
            if (!requestContent.IgnoreCache)
            {
                var getProxyCacheResult = await _mediator.Send(new GetProxyCacheHandlerModel
                {
                    RequestBodyAsKeyInput = requestBody,
                    KeyPrefix = $"{requestContent.MethodName}:{requestContent.SubMethodName}",
                });

                if (!getProxyCacheResult.IsSuccess && getProxyCacheResult.StatusCode != StatusCodes.Status404NotFound)
                {
                    _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                    {
                        ErrorMessage = getProxyCacheResult.ErrorMessage,
                        IsSuccess = getProxyCacheResult.IsSuccess,
                        StatusCode = getProxyCacheResult.StatusCode,
                        RequestData = requestBody
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
                        RequestData = requestBody,
                        ResponseData = getProxyCacheResult.Data.ResponseBody
                    }));

                    return new ContentResult
                    {
                        Content = getProxyCacheResult.Data.ResponseBody,
                        ContentType = getProxyCacheResult.Data.ContentType,
                        StatusCode = getProxyCacheResult.Data.StatusCode
                    };
                }
            }
            #endregion

            #region Get billing info
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

                #region Check notification table (call check notification service) and send notification to rabbitmq (send notification service)
                if (billingResponce.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    var limitNotificationResponce = await _mediator.Send(new LimitNotificationHandlerModel
                    {
                        BillingId = billingResponce.Data.BillingId,
                        RequestLimit = billingResponce.Data.RequestLimit,
                        RequestCount = billingResponce.Data.RequestCount,
                        BillingConfigId = billingResponce.Data.BillingConfigId
                    });

                    if (!limitNotificationResponce.IsSuccess)
                    {
                        _requestLogger.LogWarning(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                        {
                            ErrorMessage = limitNotificationResponce.ErrorMessage,
                            IsSuccess = limitNotificationResponce.IsSuccess,
                            StatusCode = limitNotificationResponce.StatusCode,
                            RequestData = requestBody
                        }));
                    }
                }
                #endregion

                return StatusCode(billingResponce.StatusCode, billingResponce.ErrorMessage);
            }
            #endregion

            #region Proxy request
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
            #endregion

            var stringContent = await proxyResponce.Data.ReadAsStringAsync();

            #region Check notification trigger and send notification
            var checkNotificationResponce = await _mediator.Send(new LimitNotificationHandlerModel
            {
                BillingId = billingResponce.Data.BillingId,
                RequestLimit = billingResponce.Data.RequestLimit,
                RequestCount = billingResponce.Data.RequestCount,
                BillingConfigId = billingResponce.Data.BillingConfigId
            });

            if (!checkNotificationResponce.IsSuccess)
            {
                _requestLogger.LogWarning(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = checkNotificationResponce.ErrorMessage,
                    IsSuccess = checkNotificationResponce.IsSuccess,
                    StatusCode = checkNotificationResponce.StatusCode,
                    RequestData = requestBody
                }));
            }
            #endregion

            #region Cache response
            var cacheResponce = await _mediator.Send(new SetProxyCacheHandlerModel
            {
                RequestBodyAsKeyInput = requestBody,
                KeyPrefix = $"{requestContent.MethodName}:{requestContent.SubMethodName}",
                ResponseBody = stringContent,
                ContentType = proxyResponce.Data.Headers.ContentType?.ToString(),
                StatusCode = proxyResponce.StatusCode
            });

            if (!cacheResponce.IsSuccess)
            {
                _requestLogger.LogWarning(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = cacheResponce.ErrorMessage,
                    IsSuccess = cacheResponce.IsSuccess,
                    StatusCode = cacheResponce.StatusCode,
                    RequestData = requestBody
                }));
            }
            #endregion

            #region Log request\response object and return response
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
            #endregion
        }
    }
}