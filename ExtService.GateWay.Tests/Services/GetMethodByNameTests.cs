using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.ServiceRequests;
using ExtService.GateWay.API.Services.SMethodInfo;
using ExtService.GateWay.DBContext.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExtService.GateWay.Tests.Services
{
    public class GetMethodByNameTests
    {
        private readonly Mock<IDBManager> _mockDbManager;
        private readonly Mock<ILogger<MethodInfoService>> _mockLogger;
        private readonly MethodInfoService _service;

        public GetMethodByNameTests()
        {
            _mockDbManager = new Mock<IDBManager>();
            _mockLogger = new Mock<ILogger<MethodInfoService>>();
            _service = new MethodInfoService(_mockDbManager.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetMethodInfoAsync_ShouldReturnNotFound_WhenMethodNotFound()
        {
            // Arrange
            _mockDbManager.Setup(m => m.MethodInfoRepository.RetrieveAsync(It.IsAny<Expression<Func<MethodInfo, bool>>>()))
                .ReturnsAsync((MethodInfo)null);

            var request = new SearchMethodRequest { MethodName = "method1" };

            // Act
            var result = await _service.GetMethodInfoAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal($"Метод с именем \"{request.MethodName}\" не найден.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetMethodInfoAsync_ShouldReturnSuccess_WhenMethodFound()
        {
            // Arrange
            var methodInfo = new MethodInfo { MethodName = "method1" };
            _mockDbManager.Setup(m => m.MethodInfoRepository.RetrieveAsync(It.IsAny<Expression<Func<MethodInfo, bool>>>()))
                .ReturnsAsync(methodInfo);

            var request = new SearchMethodRequest { MethodName = "method1" };

            // Act
            var result = await _service.GetMethodInfoAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(methodInfo, result.Data);
        }
    }
}
