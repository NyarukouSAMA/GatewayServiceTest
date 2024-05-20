using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExtService.GateWay.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GateWayControllerV1 : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GateWayControllerV1> _logger;

        public GateWayControllerV1(IMediator mediator, ILogger<GateWayControllerV1> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
                return StatusCode(identificationResponce.StatusCode, identificationResponce.ErrorMessage);
            }

            var methodInfoResponce = await _mediator.Send(new SearchMethodRequest { MethodName = HttpContext.Request.Method });
            if (!methodInfoResponce.IsSuccess)
            {
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
                return StatusCode(billingResponce.StatusCode, billingResponce.ErrorMessage);
            }

            var proxyResponce = await _mediator.Send(request);
            if (!proxyResponce.IsSuccess)
            {
                return StatusCode(proxyResponce.StatusCode, proxyResponce.ErrorMessage);
            }

            return Ok(proxyResponce.Data);

        }
    }
}
