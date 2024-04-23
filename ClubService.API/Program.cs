using Asp.Versioning;
using ClubService.API;
using ClubService.Application.Api;
using ClubService.Application.Impl;
using ClubService.Domain.Repository;
using ClubService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Read values from appsettings
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// Repositories
builder.Services.AddScoped<IEventRepository, MockEventRepository>();
builder.Services.AddSingleton<IEventPublisher>(new RedisEventPublisher(redisConnectionString));

// Services
builder.Services.AddScoped<IRegisterTennisClubService, RegisterTennisClubService>();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClubServiceV1"); });
}

app.MapControllers();
app.Run();