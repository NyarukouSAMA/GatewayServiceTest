using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.ServiceRequests;
using ExtService.GateWay.API.Services.SClientIdentification;
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
    public class CheckUserByClientIdTests
    {
        private readonly Mock<IDBManager> _mockDbManager;
        private readonly Mock<ILogger<CheckUserByClientId>> _mockLogger;
        private readonly CheckUserByClientId _service;

        public CheckUserByClientIdTests()
        {
            _mockDbManager = new Mock<IDBManager>();
            _mockLogger = new Mock<ILogger<CheckUserByClientId>>();
            _service = new CheckUserByClientId(_mockDbManager.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task IdentifyClientAsync_ShouldReturnNotFound_WhenClientNotFound()
        {
            // Arrange
            _mockDbManager.Setup(m => m.IdentificationRepository.RetrieveAsync(It.IsAny<Expression<Func<Identification, bool>>>()))
                .ReturnsAsync((Identification)null);

            var request = new ClientIdentificationRequest { ClientId = "client1" };

            // Act
            var result = await _service.IdentifyClientAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal($"Пользователь с таким ClienId \"{request.ClientId}\" не найден.", result.ErrorMessage);
        }

        [Fact]
        public async Task IdentifyClientAsync_ShouldReturnSuccess_WhenClientFound()
        {
            // Arrange
            var identification = new Identification { ClientId = "client1" };
            _mockDbManager.Setup(m => m.IdentificationRepository.RetrieveAsync(It.IsAny<Expression<Func<Identification, bool>>>()))
                .ReturnsAsync(identification);

            var request = new ClientIdentificationRequest { ClientId = "client1" };

            // Act
            var result = await _service.IdentifyClientAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(identification, result.Data);
        }
    }
}
