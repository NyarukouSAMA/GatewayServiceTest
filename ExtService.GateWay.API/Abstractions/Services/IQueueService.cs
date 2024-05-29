using ExtService.GateWay.API.Models.Common;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IQueueService
    {
        Task<ServiceResponse<bool>> PublishToQueue<TMessage>(string exchangeName, string queueName, TMessage message);
        Task<ServiceResponse<bool>> PublishToQueue(string exchangeName, string queueName, string message);
    }
}
