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

// Configure swagger
builder.ConfigureSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "ExtService.GateWay.API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
