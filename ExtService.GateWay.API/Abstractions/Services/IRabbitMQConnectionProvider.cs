using RabbitMQ.Client;

namespace ExtService.GateWay.API.Abstractions.Services
{
    public interface IRabbitMQConnectionProvider
    {
        /// <summary>
        /// Executes the action provided by the class consumer using the channel.
        /// </summary>
        /// <typeparam name="TResponse">User defined response type</typeparam>
        /// <param name="channelAction">User defined channel action</param>
        /// <returns>The execution result of the channel action</returns>
        public Task<TResponse> ExecuteChannelActionAsync<TResponse>(Func<IModel, TResponse> channelAction);
    }
}
