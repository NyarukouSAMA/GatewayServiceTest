using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.Logging;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Proxy([FromBody] ProxyRequest request)
        {
            var clientId = "testClientId";
            //var clientId = User.Claims.FirstOrDefault(c => c.Type == "azp")?.Value;
            if (string.IsNullOrEmpty(clientId))
            {
                _logger.LogError("Client id not found in token.");
                return Unauthorized();
            }

            var identificationResponce = await _mediator.Send(new ClientIdentificationRequest { ClientId = clientId });
            if (!identificationResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<ProxyRequest, string>
                {
                    ErrorMessage = identificationResponce.ErrorMessage,
                    IsSuccess = identificationResponce.IsSuccess,
                    StatusCode = identificationResponce.StatusCode,
                    RequestData = request
                }));
                return StatusCode(identificationResponce.StatusCode, identificationResponce.ErrorMessage);
            }

            var methodInfoResponce = await _mediator.Send(new SearchMethodRequest { MethodName = HttpContext.Request.Method });
            if (!methodInfoResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<ProxyRequest, string>
                {
                    ErrorMessage = methodInfoResponce.ErrorMessage,
                    IsSuccess = methodInfoResponce.IsSuccess,
                    StatusCode = methodInfoResponce.StatusCode,
                    RequestData = request
                }));
                return StatusCode(methodInfoResponce.StatusCode, methodInfoResponce.ErrorMessage);
            }

            var billingResponce = await _mediator.Send(new BillingRequest
            {
                ClientId = clientId,
                IdentificationId = identificationResponce.Data.IdentificationId,
                MethodId = methodInfoResponce.Data.MethodId,
                CurrentDate = DateTime.UtcNow
            });
            if (!billingResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<ProxyRequest, string>
                {
                    ErrorMessage = billingResponce.ErrorMessage,
                    IsSuccess = billingResponce.IsSuccess,
                    StatusCode = billingResponce.StatusCode,
                    RequestData = request
                }));
                return StatusCode(billingResponce.StatusCode, billingResponce.ErrorMessage);
            }

            var proxyResponce = await _mediator.Send(request);
            if (!proxyResponce.IsSuccess)
            {
                _requestLogger.LogError(JsonConvert.SerializeObject(new LoggingRequest<ProxyRequest, string>
                {
                    ErrorMessage = proxyResponce.ErrorMessage,
                    IsSuccess = proxyResponce.IsSuccess,
                    StatusCode = proxyResponce.StatusCode,
                    RequestData = request
                }));
                return StatusCode(proxyResponce.StatusCode, proxyResponce.ErrorMessage);
            }

            _requestLogger.LogInformation(JsonConvert.SerializeObject(new LoggingRequest<ProxyRequest, string>
            {
                ErrorMessage = proxyResponce.ErrorMessage,
                IsSuccess = proxyResponce.IsSuccess,
                StatusCode = proxyResponce.StatusCode,
                RequestData = request,
                ResponseData = proxyResponce.Data
            }));

            return Ok(proxyResponce.Data);

        }
    }
}
