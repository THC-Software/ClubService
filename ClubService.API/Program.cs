using Asp.Versioning;
using ClubService.API;
using ClubService.Application.Api;
using ClubService.Application.EventHandlers;
using ClubService.Application.EventHandlers.AdminEventHandlers;
using ClubService.Application.EventHandlers.MemberEventHandlers;
using ClubService.Application.EventHandlers.SubscriptionTierEventHandlers;
using ClubService.Application.EventHandlers.TennisClubEventHandlers;
using ClubService.Application.EventHandlers.TournamentEventHandlers;
using ClubService.Application.Impl;
using ClubService.Domain.Api;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;
using ClubService.Infrastructure;
using ClubService.Infrastructure.DbContexts;
using ClubService.Infrastructure.EventHandling;
using ClubService.Infrastructure.Repositories;
using ClubService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<EventStoreDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("event-store-connection"));
});
builder.Services.AddDbContext<ReadStoreDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("read-store-connection"));
});
builder.Services.AddDbContext<LoginStoreDbContext>(options =>
{
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("login-store-connection"));
});

// Repositories
builder.Services.AddScoped<IEventRepository, PostgresEventRepository>();
builder.Services.AddScoped<ISubscriptionTierReadModelRepository, SubscriptionTierReadModelRepository>();
builder.Services.AddScoped<ITennisClubReadModelRepository, TennisClubReadModelRepository>();
builder.Services.AddScoped<IAdminReadModelRepository, AdminReadModelRepository>();
builder.Services.AddScoped<IMemberReadModelRepository, MemberReadModelRepository>();
builder.Services.AddScoped<ITournamentReadModelRepository, TournamentReadModelRepository>();
builder.Services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();

builder.Services.AddScoped<ILoginRepository, LoginRepository>();

// Services
builder.Services.AddScoped<IRegisterMemberService, RegisterMemberService>();
builder.Services.AddScoped<IUpdateMemberService, UpdateMemberService>();
builder.Services.AddScoped<IDeleteMemberService, DeleteMemberService>();
builder.Services.AddScoped<IRegisterTennisClubService, RegisterTennisClubService>();
builder.Services.AddScoped<IUpdateTennisClubService, UpdateTennisClubService>();
builder.Services.AddScoped<IDeleteTennisClubService, DeleteTennisClubService>();
builder.Services.AddScoped<IRegisterAdminService, RegisterAdminService>();
builder.Services.AddScoped<IDeleteAdminService, DeleteAdminService>();
builder.Services.AddScoped<IUpdateAdminService, UpdateAdminService>();

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ILoginService, LoginService>();

// Transaction
builder.Services.AddScoped<IReadStoreTransactionManager, TransactionManager<ReadStoreDbContext>>();
builder.Services.AddScoped<IEventStoreTransactionManager, TransactionManager<EventStoreDbContext>>();

// Event Handler
builder.Services.AddScoped<ChainEventHandler>();
builder.Services.AddScoped<IEventHandler, SubscriptionTierCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler, TennisClubRegisteredEventHandler>();
builder.Services.AddScoped<IEventHandler, TennisClubLockedEventHandler>();
builder.Services.AddScoped<IEventHandler, TennisClubNameChangedEventHandler>();
builder.Services.AddScoped<IEventHandler, TennisClubSubscriptionTierChangedEventHandler>();
builder.Services.AddScoped<IEventHandler, TennisClubDeletedEventHandler>();
builder.Services.AddScoped<IEventHandler, TennisClubUnlockedEventHandler>();
builder.Services.AddScoped<IEventHandler, AdminRegisteredEventHandler>();
builder.Services.AddScoped<IEventHandler, AdminDeletedEventHandler>();
builder.Services.AddScoped<IEventHandler, AdminFullNameChangedEventHandler>();
builder.Services.AddScoped<IEventHandler, MemberRegisteredEventHandler>();
builder.Services.AddScoped<IEventHandler, MemberLockedEventHandler>();
builder.Services.AddScoped<IEventHandler, MemberUnlockedEventHandler>();
builder.Services.AddScoped<IEventHandler, MemberDeletedEventHandler>();
builder.Services.AddScoped<IEventHandler, MemberFullNameChangedEventHandler>();
builder.Services.AddScoped<IEventHandler, MemberEmailChangedEventHandler>();
builder.Services.AddScoped<IEventHandler, TournamentConfirmedEventHandler>();
builder.Services.AddScoped<IEventHandler, TournamentCanceledEventHandler>();

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

// TODO: Logging/errorhandling?
var redisHost = builder.Configuration["RedisConfig:Host"];
var redisStreamName = builder.Configuration["RedisConfig:StreamName"];
var redisGroupName = builder.Configuration["RedisConfig:ConsumerGroup"];
if (redisHost == null || redisStreamName == null || redisGroupName == null)
{
    Console.WriteLine("RedisConfig is not correctly configured");
}
else
{
    builder.Services.AddSingleton<IEventReader>(sp =>
    {
        var cancellationToken = new CancellationTokenSource().Token;
        return new RedisEventReader(cancellationToken, sp, redisHost, redisStreamName, redisGroupName);
    });

    builder.Services.AddHostedService<EventReaderScheduler>();
}

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDevelopment"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClubServiceV1"); });

    var eventStoreDbContext = services.GetRequiredService<EventStoreDbContext>();
    await eventStoreDbContext.Database.EnsureDeletedAsync();
    await eventStoreDbContext.Database.EnsureCreatedAsync();

    var readStoreDbContext = services.GetRequiredService<ReadStoreDbContext>();
    await readStoreDbContext.Database.EnsureDeletedAsync();
    await readStoreDbContext.Database.EnsureCreatedAsync();

    var loginStoreDbContext = services.GetRequiredService<LoginStoreDbContext>();
    await loginStoreDbContext.Database.EnsureDeletedAsync();
    await loginStoreDbContext.Database.EnsureCreatedAsync();
}

// Register event handler in ChainEventHandler
var chainEventHandler = services.GetRequiredService<ChainEventHandler>();
var eventHandlers = services.GetServices<IEventHandler>();

foreach (var eventHandler in eventHandlers)
{
    chainEventHandler.RegisterEventHandler(eventHandler);
}

app.MapControllers();
app.UseExceptionHandler();
app.Run();

// For integration tests
public abstract partial class Program
{
}