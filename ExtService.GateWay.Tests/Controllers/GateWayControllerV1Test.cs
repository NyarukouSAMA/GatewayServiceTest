using ExtService.GateWay.API.Controllers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.HandlerModels;
using ExtService.GateWay.API.Models.ServiceRequests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace ExtService.GateWay.Tests.Controllers
{
    public class GateWayControllerV1Test
    {
        private readonly GateWayControllerV1 _controller;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<GateWayControllerV1>> _loggerMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<ILogger> _mockRequestLogger;

        public GateWayControllerV1Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<GateWayControllerV1>>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _mockRequestLogger = new Mock<ILogger>();
            _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockRequestLogger.Object);

            _controller = new GateWayControllerV1(_mediatorMock.Object, _loggerMock.Object, _loggerFactoryMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Proxy_ShouldReturnUnauthorized_WhenClientIdIsEmpty()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("test request content"));
            _controller.ControllerContext.HttpContext = context;

            // Act
            var result = await _controller.Proxy();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Proxy_ShouldReturnStatusCode_WhenBillingFails()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("test request content"));
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim("azp", "testClientId") });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claimsPrincipal };

            _mediatorMock.Setup(m => m.Send(It.IsAny<BillingHandlerModel>(), default))
                .ReturnsAsync(new ServiceResponse<bool> { IsSuccess = false, StatusCode = 400, ErrorMessage = "Billing error" });

            // Act
            var result = await _controller.Proxy();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, statusCodeResult.StatusCode);
            Assert.Equal("Billing error", statusCodeResult.Value);
        }

        [Fact]
        public async Task Proxy_ShouldReturnOk_WhenAllProcessesSucceed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("test request content"));
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim("azp", "testClientId") });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claimsPrincipal };

            _mediatorMock.Setup(m => m.Send(It.IsAny<BillingHandlerModel>(), default))
                .ReturnsAsync(new ServiceResponse<bool> { IsSuccess = true });

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProxyRequest>(), default))
                .ReturnsAsync(new ServiceResponse<string> { IsSuccess = true, Data = "response data" });

            // Act
            var result = await _controller.Proxy();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("response data", okResult.Value);
        }
    }
}
