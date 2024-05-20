﻿using ExtService.GateWay.API.Abstractions.Factories;
using ExtService.GateWay.API.Abstractions.Repositories;
using ExtService.GateWay.API.Abstractions.Resolvers;
using ExtService.GateWay.API.Abstractions.Strategy;
using ExtService.GateWay.API.Abstractions.UnitsOfWork;
using ExtService.GateWay.API.Models.Options;
using ExtService.GateWay.API.Services.Repositories;
using ExtService.GateWay.API.Strategies.Factories;
using ExtService.GateWay.API.Strategies.SBilling;
using ExtService.GateWay.API.Strategies.SClientIdentification;
using ExtService.GateWay.API.Strategies.SMethodInfo;
using ExtService.GateWay.API.Strategies.SProxing;
using ExtService.GateWay.API.Utilities.DBUtils;
using ExtService.GateWay.API.Utilities.Resolvers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
using System.Security.Cryptography;

namespace ExtService.GateWay.API.Helpers
{
    public static class BuilderExtentions
    {
        public static WebApplicationBuilder RegisterOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<KeyCloakOptions>(builder.Configuration.GetSection(KeyCloakOptions.KeyCloakConfigSection));
            builder.Services.Configure<MockupOptions>(builder.Configuration.GetSection(MockupOptions.MockupOptionsSection));
            return builder;
        }

        public static WebApplicationBuilder RegisterCommonServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient();
            
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
                            IssuerSigningKey = BuildRSAKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA8YRVL+Jssv2jcG9cChkzpGJyXEUShraJ76xtZMx7wQg/Rz8GOHaXFSRiCPxH5Vuo+enQrtotxvUQBZSX6KpPBF5HUfXzy04R0ybtLfXcaUstU5emDuLmCHtnRmlDMv01AXC5StTq2XUJDUIN0BdWIBpVb9HM7LVrj1l7zwlCNVSxAPrRL2U+YS1WNuF0NGsaI0NNQ43OX4KkgWdRXTzcWXiSUUXKsZdu0zkmlpFo8C1FXp2Q7CVN/R5cGKAcMyKRLRMiHcznZ0IVQBxqAuHTVN5oo4+egktaavKQQEjVPckuoCvEzxN7WCZlUWgwChBR+dP1oyYKNRGR9TuxhBBYewIDAQAB"),
                            ValidateIssuer = true,
                            ValidIssuers = new[] { keyCloakOptions.KeyCloakRealmAuthority },
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
            builder.Services.AddScoped<ProxyMockup>();
            builder.Services.AddScoped<ServiceProxing>();

            // Register factories
            builder.Services.AddScoped<IBillingStrategyFactory, BillingStrategyFactory>();
            builder.Services.AddScoped<IClientIdentificationStrategyFactory, ClientIdentificationStrategyFactory>();
            builder.Services.AddScoped<ISearchMethodStrategyFactory, SearchMethodStrategyFactory>();
            builder.Services.AddScoped<IProxingStrategyFactory, ProxingStrategyFactory>();

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
