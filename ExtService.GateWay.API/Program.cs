using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;
using ExtService.GateWay.API;
using ExtService.GateWay.API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Register options
builder.RegisterOptions();

// Register loggers
builder.RegisterLoggers();

// Register JWT token authentication
builder.RegisterJWTTokenAuth();

// Register DBService
builder.RegisterDBService();

// Register RabbitMQ service
builder.RegisterQueueService();

// Register Redis Cache service
builder.RegisterCacheService();

// Register MediatR
builder.RegisterMediatR();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configures the API versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.ReportApiVersions = true;
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc().AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});

// Register controllers using NewtonsoftJson as serialization provider
builder.Services.AddControllers().AddNewtonsoftJson();

// Configure swagger
// builder.ConfigureSwaggerGen();
// Add swagger generation
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<GateWaySwaggerGenOptions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        foreach (var description in app.Services.GetRequiredService<IApiVersionDescriptionProvider>().ApiVersionDescriptions)
        {
            opt.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
