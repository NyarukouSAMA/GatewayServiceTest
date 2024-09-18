using ExtService.Correspondence.Models.HandlerModels;

namespace ExtService.Correspondence.Abstractions.Services
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(EmailQueueMessage emailMessage);
    }
}
