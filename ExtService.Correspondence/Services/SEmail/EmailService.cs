using ExtService.Correspondence.Abstractions.Services;
using ExtService.Correspondence.Models.HandlerModels;
using ExtService.Correspondence.Models.Options;
using MailKit.Net.Smtp;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ExtService.Correspondence.Services.SEmail
{
    public class EmailService : IEmailService
    {
        private readonly EmailExchangeOptions _emailOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailExchangeOptions> emailOptions,
            ILogger<EmailService> logger)
        {
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailQueueMessage emailMessage)
        {
            try
            {
                var message = new MimeMessage();

                // Set the message sender
                message.From.Add(new MailboxAddress(_emailOptions.FromDisplayName, _emailOptions.FromEmailAddress));

                // Set the recipients
                var recipients = emailMessage.RecipientListSemikolonSeparated
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(email => email.Trim());

                foreach (var recipient in recipients)
                {
                    message.To.Add(MailboxAddress.Parse(recipient));
                }

                // Set the subject
                message.Subject = emailMessage.Subject;

                // Create the body
                var bodyBuilder = new BodyBuilder();

                if (emailMessage.BodyType == 1)
                {
                    bodyBuilder.TextBody = emailMessage.Message;
                }
                else
                {
                    bodyBuilder.HtmlBody = emailMessage.Message;
                }

                message.Body = bodyBuilder.ToMessageBody();

                // Optionally BCC for saving email copies
                if (_emailOptions.SaveEmailCopy &&
                    !string.IsNullOrWhiteSpace(_emailOptions.SaveCopyEmailAddress))
                {
                    message.Bcc.Add(MailboxAddress.Parse(_emailOptions.SaveCopyEmailAddress));
                }

                using var smtpClient = new SmtpClient();

                // Accept all SSL certificates (for self-signed certificates)
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Connect to the SMTP server
                await smtpClient.ConnectAsync(
                    _emailOptions.SmtpServer,
                    _emailOptions.SmtpPort,
                    _emailOptions.UseSsl);

                // Authenticate if necessary
                if (_emailOptions.UseAuthentication)
                {
                    await smtpClient.AuthenticateAsync(
                        _emailOptions.SmtpUser,
                        _emailOptions.SmtpPassword);
                }

                // Send the email
                await smtpClient.SendAsync(message);

                // Disconnect from the SMTP server
                await smtpClient.DisconnectAsync(true);

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
