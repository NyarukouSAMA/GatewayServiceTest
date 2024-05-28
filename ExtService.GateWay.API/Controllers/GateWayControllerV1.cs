using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.HandlerModels;
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

            var billingResponce = await _mediator.Send(new BillingHandlerModel
            {
                ClientId = clientId,
                MethodName = MethodConstants.SuggestionMethodName,
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
