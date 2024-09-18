using MediatR;

namespace MSP.QueueClient.Models
{
    public class QueueMessage<TData> : Message<TData>, IRequest<bool>
    {
    }
}
