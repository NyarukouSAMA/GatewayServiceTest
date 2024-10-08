﻿using ExtService.GateWay.API.Abstractions.Services;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Helpers;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.ServiceModels;
using ExtService.GateWay.DBContext.DBModels;

namespace ExtService.GateWay.API.Services.SClientIdentification
{
    public class ClientIdentificationService : IClientIdentificationService
    {
        private readonly IDBManager _dbManager;
        private readonly ILogger<ClientIdentificationService> _logger;

        public ClientIdentificationService(IDBManager dbManager,
            ILogger<ClientIdentificationService> logger)
        {
            _dbManager = dbManager;
            _logger = logger;
        }
        public async Task<ServiceResponse<Identification>> IdentifyClientAsync(ClientIdentificationRequest clientIdentificationRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                Identification identification = await _dbManager?.IdentificationRepository
                    ?.RetrieveAsync(identification => identification.ClientId == clientIdentificationRequest.ClientId,
                    new System.Linq.Expressions.Expression<Func<Identification, object>>[]
                    {
                        identification => identification.SystemInfo
                    });

                if (identification == null)
                {
                    string errorMessage = $"Пользователь с таким ClienId \"{clientIdentificationRequest.ClientId}\" не найден.";
                    _logger.LogError(errorMessage);

                    return new ServiceResponse<Identification>
                    {
                        IsSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = errorMessage
                    };
                }

                return new ServiceResponse<Identification>
                {
                    IsSuccess = true,
                    Data = identification
                };
            }
            catch (Exception ex)
            {
                string headerMessage = "Во время поиска пользователя возникла непредвиденная ошибка.";

                _logger.LogError(ex, headerMessage);
                return new ServiceResponse<Identification>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = ex.BuildExceptionMessage(headerMessage)
                };
            }
        }
    }
}
