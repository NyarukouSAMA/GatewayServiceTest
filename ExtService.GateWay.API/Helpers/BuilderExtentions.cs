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
using System.Security.Cryptography;
using ExtService.GateWay.API.Abstractions.Services;
using Microsoft.OpenApi.Models;
using ExtService.GateWay.API.Services.SQueue;
using ExtService.GateWay.API.Services.SCache;
using ExtService.GateWay.API.Services.SLimitCheck;

namespace ExtService.GateWay.API.Helpers
{
    public static class BuilderExtentions
    {
        public static WebApplicationBuilder RegisterOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<KeyCloakOptions>(builder.Configuration.GetSection(KeyCloakOptions.KeyCloakConfigSection));
            builder.Services.Configure<MockupOptions>(builder.Configuration.GetSection(MockupOptions.MockupOptionsSection));
            builder.Services.Configure<ProxyOptions>(builder.Configuration.GetSection(ProxyOptions.ProxyOptionsSection));
            builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(RabbitMQOptions.RabbitMQOptionsSection));
            builder.Services.Configure<NotificationExchangeOptions>(builder.Configuration.GetSection(NotificationExchangeOptions.NotificationExchangeOptionsSection));
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
            builder.Services.AddScoped<IBillingConfigRepository, BillingConfigRepository>();
            builder.Services.AddScoped<IBillingRepository, BillingRepository>();
            builder.Services.AddScoped<IMethodInfoRepository, MethodInfoRepository>();
            builder.Services.AddScoped<INotificationInfoRepository, NotificationInfoRepository>();

            builder.Services.AddScoped<IDBManager, DBManager>();

            return builder;
        }

        public static WebApplicationBuilder RegisterQueueService(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IRabbitMQConnectionProvider, RabbitMQConnectionProvider>();

            builder.Services.AddTransient<IRabbitMQPublisherService, RabbitMQPublisherService>();
            return builder;
        }

        public static WebApplicationBuilder RegisterCacheService(this WebApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "ExtService.GateWay.API";
            });

            builder.Services.AddScoped<ICacheService, RedisCacheService>();
            return builder;
        }

        public static WebApplicationBuilder RegisterMediatR(this WebApplicationBuilder builder)
        {
            // Register billing handler strategies
            builder.Services.AddScoped<BillingService>();
            builder.Services.AddScoped<BillingServiceMockup>();

            // Register client identification strategies
            builder.Services.AddScoped<ClientIdentificationMockup>();
            builder.Services.AddScoped<ClientIdentificationService>();

            // Register search method strategies
            builder.Services.AddScoped<MethodInfoMockup>();
            builder.Services.AddScoped<MethodInfoService>();

            // Register proxing strategies
            builder.Services.AddKeyedTransient<IProxingService, ProxyMockup>(ServiceNames.ProxingMockupName);
            builder.Services.AddKeyedTransient<IProxingService, ServiceProxing>(ServiceNames.ProxingServiceName);

            builder.Services.AddKeyedTransient< IProxyContentTransformer, GetProxyContentTransformer>(HttpMethod.Get.Method);
            builder.Services.AddKeyedTransient<IProxyContentTransformer, PostProxyContentTransformer>(HttpMethod.Post.Method);

            // Register limit check strategies
            builder.Services.AddTransient<LimitCheckMockup>();
            builder.Services.AddTransient<LimitCheckService>();

            // Register factories
            builder.Services.AddSingleton<IBillingServiceFactory, BillingServiceFactory>();
            builder.Services.AddSingleton<IClientIdentificationServiceFactory, ClientIdentificationServiceFactory>();
            builder.Services.AddSingleton<ISearchMethodServiceFactory, SearchMethodServiceFactory>();
            builder.Services.AddSingleton<IProxingServiceFactory, ProxingServiceFactory>();
            builder.Services.AddSingleton<ILimitCheckServiceFactory, LimitCheckServiceFactory>();
            builder.Services.AddSingleton<IRestProxyContentTransformerFactory, RestProxyContentTransformerFactory>();
            builder.Services.AddSingleton<IPluginFactory, PluginFactory>();

            // Register handlers
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
            return builder;
        }

        public static WebApplicationBuilder ConfigureSwaggerGen(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ExtService.GateWay.API",
                    Version = "v1",
                    Description = "Gateway service v 1.0 - base functionality",
                });

                opt.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "ExtService.GateWay.API",
                    Version = "v2",
                    Description = "Gateway service v 2.0 - added caching and notification functionality",
                });

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

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
