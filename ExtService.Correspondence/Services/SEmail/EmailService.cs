using ExtService.Correspondence.Abstractions.Services;
using ExtService.Correspondence.Models.HandlerModels;
using ExtService.Correspondence.Models.Options;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Options;

namespace ExtService.Correspondence.Services.SEmail
{
    public class EmailService : IEmailService
    {
        private readonly ExchangeService _exchangeService;
        private readonly EmailExchangeOptions _emailOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ExchangeService exchangeService,
            IOptions<EmailExchangeOptions> emailOptions,
            ILogger<EmailService> logger)
        {
            _exchangeService = exchangeService;
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailQueueMessage emailMessage)
        {
            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    var email = new EmailMessage(_exchangeService)
                    {
                        Subject = emailMessage.Subject,
                        Body = new MessageBody(emailMessage.BodyType == 1 ? BodyType.Text : BodyType.HTML, emailMessage.Message)
                    };

                    email.ToRecipients
                        .AddRange(emailMessage.RecipientListSemikolonSeparated.Split(';')
                        .Select(recipientAddress => recipientAddress.Trim()));

                    if (_emailOptions.SaveEmailCopy)
                    {
                        email.SendAndSaveCopy(_emailOptions.SaveCopyFolderWellKnownName);
                    }
                    else
                    {
                        email.Send();
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return false;
            }
        }
    }
}
