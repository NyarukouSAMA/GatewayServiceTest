using ExtService.GateWay.API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Register options
builder.RegisterOptions();

// Register common services
builder.RegisterCommonServices();

// Register JWT token authentication
builder.RegisterJWTTokenAuth();

// Register DBService
builder.RegisterDBService();

// Register MediatR
builder.RegisterMediatR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
