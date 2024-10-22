using ExtService.Correspondence.Abstractions.Services;
using ExtService.Correspondence.Models.HandlerModels;
using ExtService.Correspondence.Models.Options;
using ExtService.Correspondence.Services.SEmail;
using Microsoft.Exchange.WebServices.Data;
using MSP.QueueClient.Models.Options;
using MSP.QueueClient.DI.AspNet;

namespace ExtService.Correspondence
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

                    services.Configure<ServiceQueues>(hostContext.Configuration.GetSection(ServiceQueues.ServiceQueuesSection));
                    services.Configure<EmailExchangeOptions>(hostContext.Configuration.GetSection(EmailExchangeOptions.EmailExchangeOptionsSection));
                    services.Configure<RabbitMQOptions>(hostContext.Configuration.GetSection(RabbitMQOptions.RabbitMQOptionsSection));
                    services.Configure<MockupOptions>(hostContext.Configuration.GetSection(MockupOptions.MockupOptionsSection));

                    var configuration = hostContext.Configuration;

                    var queueNames = configuration
                        .GetSection("ServiceQueues")
                        .Get<ServiceQueues>();

                    services.AddRabbitMQSingletonConfigurator();
                    services.AddRabbitMQSingletonConsumer<EmailQueueMessage>(queueNames.EmailQueueName);

                    var exchangeOptions = configuration
                        .GetSection(EmailExchangeOptions.EmailExchangeOptionsSection)
                        .Get<EmailExchangeOptions>();

                    services.AddScoped<IEmailService, EmailService>();

                    services.AddHostedService<ConsumerWorker>();
                })
                .Build();

            host.Run();
        }
    }
}