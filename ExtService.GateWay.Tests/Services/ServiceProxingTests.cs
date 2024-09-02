using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.API.Services.SProxing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using System.Net;
using Xunit;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.Tests.Services
{
    public class ServiceProxingTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ILogger<ServiceProxing>> _mockLogger;
        private readonly ServiceProxing _service;

        public ServiceProxingTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockLogger = new Mock<ILogger<ServiceProxing>>();

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _service = new ServiceProxing(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnSuccess_WhenHttpRequestSucceeds()
        {
            // Arrange
            var request = new ProxyRequest
            {
                ApiTimeout = 10,
                Method = HttpMethod.Post,
                Body = "test body",
                RequestUri = "http://localhost/test",
                MethodHeaders = new List<MethodHeaders>() 
                { 
                    new MethodHeaders { HeaderName = "media-type", HeaderValue = "application/json" }
                }
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("response data")
                });

            // Act
            var serviceResponce = await _service.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.True(serviceResponce.IsSuccess);

            var result = await serviceResponce.Data.ReadAsStringAsync();
            Assert.Equal("response data", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenHttpRequestFails()
        {
            // Arrange
            var request = new ProxyRequest
            {
                ApiTimeout = 10,
                Method = HttpMethod.Post,
                Body = "test body",
                RequestUri = "http://localhost/test",
                MethodHeaders = new List<MethodHeaders>()
                {
                    new MethodHeaders { HeaderName = "media-type", HeaderValue = "application/json" }
                }
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("error message")
                });

            // Act
            var result = await _service.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Contains("error message", result.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnBadRequest_WhenMethodNameIsInvalid()
        {
            // Arrange
            var request = new ProxyRequest
            {
                ApiTimeout = 10,
                Method = HttpMethod.Post,
                Body = "test body",
                RequestUri = "http://localhost/test",
                MethodHeaders = new List<MethodHeaders>()
                {
                    new MethodHeaders { HeaderName = "media-type", HeaderValue = "application/json" }
                }
            };

            // Act
            var result = await _service.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("Неправильное имя метода: invalidMethod", result.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnServerError_WhenExceptionThrown()
        {
            // Arrange
            var request = new ProxyRequest
            {
                ApiTimeout = 10,
                Method = HttpMethod.Post,
                Body = "test body",
                RequestUri = "http://localhost/test",
                MethodHeaders = new List<MethodHeaders>()
                {
                    new MethodHeaders { HeaderName = "media-type", HeaderValue = "application/json" }
                }
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Request exception"));

            // Act
            var result = await _service.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Equal("Во время исполнения проксирующего запроса возникла непредвиденная ошибка.", result.ErrorMessage);
        }
    }
}
