using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.HandlerResponses;
using ExtService.GateWay.API.Models.Logging;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExtService.GateWay.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GateWayControllerV1 : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GateWayControllerV1> _logger;
        private readonly ILogger _requestLogger;

        public GateWayControllerV1(IMediator mediator,
            ILogger<GateWayControllerV1> logger,
            ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _requestLogger = loggerFactory.CreateLogger(LoggingConstants.RequestLogCategory);
        }

        [HttpPost("proxy")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Proxy()
        {
            var clientId = User.Claims.FirstOrDefault(c => c.Type == "azp")?.Value;
            if (string.IsNullOrEmpty(clientId))
            {
                _logger.LogError("Client id not found in token.");
                return Unauthorized();
            }

            string requestContent = await new StreamReader(Request.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestContent))
            {
                _logger.LogError("Request body is empty.");
                return BadRequest("Request body is empty.");
            }

            var identificationResponce = await _mediator.Send(new IdentificationHandlerModel
            {
                ClientId = clientId,
                MethodName = MethodConstants.SuggestionMethodName
            });

            if (!identificationResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = identificationResponce.ErrorMessage,
                    IsSuccess = identificationResponce.IsSuccess,
                    StatusCode = identificationResponce.StatusCode,
                    RequestData = requestContent
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
                IdentificationId = identificationResponce.Data.IdentificationId,
                ClientId = clientId,
                MethodId = identificationResponce.Data.MethodId,
                CurrentDate = DateTime.UtcNow
            });

            if (!billingResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = billingResponce.ErrorMessage,
                    IsSuccess = billingResponce.IsSuccess,
                    StatusCode = billingResponce.StatusCode,
                    RequestData = requestContent
                }));

                return StatusCode(billingResponce.StatusCode, billingResponce.ErrorMessage);
            }

            var request = new ProxyRequest
            {
                RequestMethodName = MethodConstants.SuggestionMethodName,
                Method = HttpMethod.Post,
                RequestPath = HttpContext.Request.Path,
                Body = requestContent
            };

            var proxyResponce = await _mediator.Send(request);
            if (!proxyResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<string, string>
                {
                    ErrorMessage = proxyResponce.ErrorMessage,
                    IsSuccess = proxyResponce.IsSuccess,
                    StatusCode = proxyResponce.StatusCode,
                    RequestData = requestContent
                }));
                return StatusCode(proxyResponce.StatusCode, proxyResponce.ErrorMessage);
            }

            //There we need to save everything to cache

            _requestLogger.LogInformation(JsonConvert.SerializeObject(new LoggingRequest<string, string>
            {
                ErrorMessage = proxyResponce.ErrorMessage,
                IsSuccess = proxyResponce.IsSuccess,
                StatusCode = proxyResponce.StatusCode,
                RequestData = requestContent,
                ResponseData = proxyResponce.Data
            }));

            return Ok(proxyResponce.Data);

        }
    }
}
