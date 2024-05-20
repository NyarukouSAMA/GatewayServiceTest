using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.Common;
using ExtService.GateWay.API.Models.DBModels;
using ExtService.GateWay.API.Models.ServiceRequests;

namespace ExtService.GateWay.API.Strategies.SClientIdentification
{
    public class CheckUserByClientId : IClientIdentificationStrategy
    {
        private readonly IDBManager _dbManager;
        private readonly ILogger<CheckUserByClientId> _logger;

        public CheckUserByClientId(IDBManager dbManager,
            ILogger<CheckUserByClientId> logger)
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
                    ?.RetrieveAsync(identification => identification.ClientId == clientIdentificationRequest.ClientId);

                if (identification == null)
                {
                    string errorMessage = $"Client with the given ClienId {clientIdentificationRequest.ClientId} not found.";
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
                _logger.LogError(ex, "An error occurred when fetching client identification.");
                return new ServiceResponse<Identification>
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorMessage = $"An error occurred when fetching client identification. {ex.Message}"
                };
            }
        }
    }
}
