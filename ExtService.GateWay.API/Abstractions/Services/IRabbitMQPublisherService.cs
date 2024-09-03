using ExtService.GateWay.API.Models.Common;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IRabbitMQPublisherService
    {
        Task<ServiceResponse<bool>> PublishToQueue<TMessage>(string exchangeName, string routingKey, TMessage message);
    }
}
