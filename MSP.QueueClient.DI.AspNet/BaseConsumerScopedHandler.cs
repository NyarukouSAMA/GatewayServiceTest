using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MSP.QueueClient.Models;

namespace MSP.QueueClient.DI.AspNet
{
    public abstract class BaseConsumerScopedHandler<TData> : IRequestHandler<QueueMessage<TData>, bool>
    {
        private readonly IServiceProvider _serviceProvider;

        public BaseConsumerScopedHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> Handle(QueueMessage<TData> queueMessage, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return await HandleMessageAsync(queueMessage, scope.ServiceProvider, cancellationToken);
            }
        }

        protected abstract Task<bool> HandleMessageAsync(Message<TData> queueMessage, IServiceProvider scopedProvider, CancellationToken cancellationToken);
    }
}
