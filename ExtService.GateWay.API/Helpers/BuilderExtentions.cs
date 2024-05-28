using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Abstractions.Resolvers;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Constants;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Services.Repositories;
using ExtService.GateWay.API.Services.Factories;
using ExtService.GateWay.API.Services.SBilling;
using ExtService.GateWay.API.Services.SClientIdentification;
using ExtService.GateWay.API.Services.SMethodInfo;
using ExtService.GateWay.API.Services.SProxing;
using ExtService.GateWay.API.Utilities.DBUtils;
using ExtService.GateWay.API.Utilities.LoggerProviders;
using ExtService.GateWay.API.Utilities.Resolvers;
using ExtService.GateWay.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Serilog;
using Serilog.Settings.Configuration;
using System.Data;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace ExtService.GateWay.API.Helpers
{
    public static class BuilderExtentions
    {
        public static WebApplicationBuilder RegisterOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<KeyCloakOptions>(builder.Configuration.GetSection(KeyCloakOptions.KeyCloakConfigSection));
            builder.Services.Configure<MockupOptions>(builder.Configuration.GetSection(MockupOptions.MockupOptionsSection));
            builder.Services.Configure<ProxyOptions>(builder.Configuration.GetSection(ProxyOptions.ProxyOptionsSection));
            return builder;
        }

        public static WebApplicationBuilder RegisterLoggers(this WebApplicationBuilder builder)
        {
            var seriLogLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            var requestLogLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions { 
                    SectionName = LoggingConstants.RequestLogCategory
                }).CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new BaseSerilogProvider(seriLogLogger))
                .AddFilter<BaseSerilogProvider>((category, level) => category != LoggingConstants.RequestLogCategory);

            builder.Logging.AddProvider(new RequestLogProvider(requestLogLogger))
                .AddFilter<RequestLogProvider>((category, level) => category == LoggingConstants.RequestLogCategory);

            return builder;
        }

        public static WebApplicationBuilder RegisterHttpServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var proxyOptions = builder.Configuration.GetSection(ProxyOptions.ProxyOptionsSection).Get<ProxyOptions>();

            builder.Services.AddHttpClient(HTTPConstants.SuggestionApiClientName, client =>
            {
                client.BaseAddress = new Uri(proxyOptions.SuggestionApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(proxyOptions.SuggestionApiTimeoutInSeconds);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", proxyOptions.SuggestionApiToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient(HTTPConstants.CleanerApiClientName, client =>
            {
                client.BaseAddress = new Uri(proxyOptions.CleanerApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(proxyOptions.CleanerApiTimeoutInSeconds);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", proxyOptions.CleanerApiToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return builder;
        }

        public static WebApplicationBuilder RegisterJWTTokenAuth(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<ISigningKeyResolver, KeyCloakKeyResolver>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var keyCloakOptions = builder.Configuration.GetSection(KeyCloakOptions.KeyCloakConfigSection).Get<KeyCloakOptions>();
                    var mockupOptions = builder.Configuration.GetSection(MockupOptions.MockupOptionsSection).Get<MockupOptions>();
                    var keyCloakKeyResolver = builder.Services.BuildServiceProvider().GetService<ISigningKeyResolver>();

                    if (mockupOptions.MockupJWTRegistration)
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) => keyCloakKeyResolver.GetSigningKey(token, securityToken, kid, validationParameters),
                            ValidateIssuer = true,
                            ValidIssuers = new[] {
                                "http://localhost:8080/realms/testrealm",
                                "http://host.docker.internal:8080/realms/testrealm"
                            },
                            ValidateAudience = false,
                            ValidateLifetime = false
                        };
                    }
                    else
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) => keyCloakKeyResolver.GetSigningKey(token, securityToken, kid, validationParameters),
                            ValidateIssuer = true,
                            ValidIssuer = keyCloakOptions.KeyCloakRealmAuthority,
                            ValidateAudience = false,
                            ValidateLifetime = true
                        };
                    }
                });

            return builder;
        }

        public static WebApplicationBuilder RegisterDBService(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<GateWayContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddTransient<IDbConnection>(sp =>
            {
                var connection = new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
                connection.Open();
                return connection;
            });

            builder.Services.AddScoped<IIdentificationRepository, IdentificationRepository>();
            builder.Services.AddScoped<IBillingRepository, BillingRepository>();
            builder.Services.AddScoped<IMethodInfoRepository, MethodInfoRepository>();

            builder.Services.AddScoped<IDBManager, DBManager>();

            return builder;
        }

        public static WebApplicationBuilder RegisterMediatR(this WebApplicationBuilder builder)
        {
            // Register billing handler strategies
            builder.Services.AddScoped<CheckAndIncrementCounter>();
            builder.Services.AddScoped<BillingServiceMockup>();

            // Register client identification strategies
            builder.Services.AddScoped<ClientIdentificationMockup>();
            builder.Services.AddScoped<CheckUserByClientId>();

            // Register search method strategies
            builder.Services.AddScoped<MethodInfoMockup>();
            builder.Services.AddScoped<GetMethodByName>();

            // Register proxing strategies
            builder.Services.AddTransient<ProxyMockup>();
            builder.Services.AddTransient<ServiceProxing>();

            // Register factories
            builder.Services.AddSingleton<IBillingServiceFactory, BillingServiceFactory>();
            builder.Services.AddSingleton<IClientIdentificationServiceFactory, ClientIdentificationServiceFactory>();
            builder.Services.AddSingleton<ISearchMethodServiceFactory, SearchMethodServiceFactory>();
            builder.Services.AddSingleton<IProxingServiceFactory, ProxingServiceFactory>();

            // Register handlers
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
            return builder;
        }

        private static RsaSecurityKey BuildRSAKey(string publicKeyJWT)
        {
            RSA rsa = RSA.Create();

            rsa.ImportSubjectPublicKeyInfo(

                source: Convert.FromBase64String(publicKeyJWT),
                bytesRead: out _
            );

            var IssuerSigningKey = new RsaSecurityKey(rsa);

            return IssuerSigningKey;
        }
    }
}
