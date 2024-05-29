using MediatR;

namespace ExtService.GateWay.API.Handlers
{
    public class IdentificationHandler : IRequestHandler<IdentificationHandlerModel, ServiceResponse<bool>>
    {
    }
}
