using Dapper;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.API.Services.SBilling;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;
using System.Linq.Expressions;
using Xunit;
using Moq.Dapper;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.Tests.Services
{
    public class CheckAndIncrementCounterTests
    {
        private readonly Mock<IDBManager> _mockDbManager;
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly Mock<ILogger<BillingService>> _mockLogger;
        private readonly BillingService _service;

        public CheckAndIncrementCounterTests()
        {
            _mockDbManager = new Mock<IDBManager>();
            _mockConnection = new Mock<IDbConnection>();
            _mockLogger = new Mock<ILogger<BillingService>>();
            _service = new BillingService(_mockDbManager.Object, _mockConnection.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UpdateBillingRecordAsync_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _service.UpdateBillingRecordAsync(null, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("Запрос не может быть пустым.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateBillingRecordAsync_ShouldReturnNotFound_WhenBillingRecordNotFound()
        {
            // Arrange
            _mockDbManager.Setup(m => m.BillingRepository.RetrieveAsync(It.IsAny<Expression<Func<Billing, bool>>>()))
                .ReturnsAsync((Billing)null);

            var request = new BillingRequest {
                ClientId = "client1",
                IdentificationId = Guid.NewGuid(),
                MethodId = Guid.NewGuid(),
                CurrentDate = DateTime.UtcNow
            };

            // Act
            var result = await _service.UpdateBillingRecordAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal($"Биллинговая запись с параметрами clientId: \"{request.ClientId}\" and methodId: \"{request.MethodId}\" не найдена.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateBillingRecordAsync_ShouldReturnTooManyRequests_WhenRequestCountExceeded()
        {
            // Arrange
            _mockDbManager.Setup(m => m.BillingRepository.RetrieveAsync(It.IsAny<Expression<Func<Billing, bool>>>()))
                .ReturnsAsync(new Billing
                {
                    BillingId = Guid.NewGuid(),
                    IdentificationId = Guid.NewGuid(),
                    MethodId = Guid.NewGuid(),
                    RequestCount = 10,
                    RequestLimit = 10,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1)
                });

            _mockConnection.SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(),
                null, null, null, null)).ReturnsAsync(0);

            var request = new BillingRequest
            {
                ClientId = "client1",
                IdentificationId = Guid.NewGuid(),
                MethodId = Guid.NewGuid(),
                CurrentDate = DateTime.UtcNow
            };

            // Act
            var result = await _service.UpdateBillingRecordAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status429TooManyRequests, result.StatusCode);
            Assert.Equal($"Лимит запросов для clientId: \"{request.ClientId}\" по методу methodId: \"{request.MethodId}\" исчерпан.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateBillingRecordAsync_ShouldReturnSuccess_WhenRequestIsValid()
        {
            // Arrange
            _mockDbManager.Setup(m => m.BillingRepository.RetrieveAsync(It.IsAny<Expression<Func<Billing, bool>>>()))
                .ReturnsAsync(new Billing
                {
                    BillingId = Guid.NewGuid(),
                    IdentificationId = Guid.NewGuid(),
                    MethodId = Guid.NewGuid(),
                    RequestCount = 10,
                    RequestLimit = 10,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1)
                });

            _mockConnection.SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(),
                null, null, null, null)).ReturnsAsync(1);

            var request = new BillingRequest
            {
                ClientId = "client1",
                IdentificationId = Guid.NewGuid(),
                MethodId = Guid.NewGuid(),
                CurrentDate = DateTime.UtcNow
            };

            // Act
            var result = await _service.UpdateBillingRecordAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
