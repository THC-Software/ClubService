using Asp.Versioning;
using ClubService.API;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Impl;
using ClubService.Domain.Repository;
using ClubService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<EventStoreDbContext>(options =>
{
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("event-store-connection"));
});

// Repositories
builder.Services.AddScoped<IEventRepository, PostgresEventRepository>();

// Services
builder.Services.AddScoped<IRegisterMemberService, RegisterMemberService>();
builder.Services.AddScoped<IUpdateMemberService, UpdateMemberService>();
builder.Services.AddScoped<IDeleteMemberService, DeleteMemberService>();
builder.Services.AddScoped<IRegisterTennisClubService, RegisterTennisClubService>();
builder.Services.AddScoped<IUpdateTennisClubService, UpdateTennisClubService>();
builder.Services.AddScoped<IDeleteTennisClubService, DeleteTennisClubService>();
builder.Services.AddScoped<IRegisterAdminService, RegisterAdminService>();
builder.Services.AddScoped<IDeleteAdminService, DeleteAdminService>();

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

// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDevelopment"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClubServiceV1"); });
    
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<EventStoreDbContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

app.MapControllers();
app.UseExceptionHandler();
app.Run();

// For integration tests
public abstract partial class Program
{
}