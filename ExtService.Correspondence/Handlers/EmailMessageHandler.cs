using ExtService.Correspondence.Abstractions.Services;
using ExtService.Correspondence.Models.HandlerModels;
using ExtService.Correspondence.Models.Options;
using Microsoft.Extensions.Options;
using MSP.QueueClient.DI.AspNet;
using MSP.QueueClient.Models;

namespace ExtService.Correspondence.Handlers
{
    public class EmailMessageHandler : BaseConsumerScopedHandler<EmailQueueMessage>
    {
        private readonly MockupOptions _mockupOptions;
        private readonly ILogger<EmailMessageHandler> _logger;

        public EmailMessageHandler(IServiceProvider serviceProvider,
            ILogger<EmailMessageHandler> logger,
            IOptions<MockupOptions> mockupOptions) : base(serviceProvider)
        {
            _mockupOptions = mockupOptions.Value;
            _logger = logger;
        }

        protected override async Task<bool> HandleMessageAsync(Message<EmailQueueMessage> queueMessage, IServiceProvider scopedProvider, CancellationToken cancellationToken)
        {
            if (_mockupOptions.EmailServiceMockup)
            {
                _logger.LogInformation("Email message is caught and logged. It wasn't sent because EmailServiceMockup is enabled.");
                return await Task.FromResult(true);
            }

            var emailService = scopedProvider.GetRequiredService<IEmailService>();

            return await emailService.SendEmailAsync(queueMessage.Data);
        }
    }
}
