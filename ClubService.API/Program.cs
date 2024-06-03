using Asp.Versioning;
using ClubService.API;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.EventHandlers;
using ClubService.Application.EventHandlers.AdminEventHandlers;
using ClubService.Application.EventHandlers.MemberEventHandlers;
using ClubService.Application.EventHandlers.SubscriptionTierEventHandlers;
using ClubService.Application.EventHandlers.TennisClubEventHandlers;
using ClubService.Application.Impl;
using ClubService.Domain.Repository;
using ClubService.Infrastructure;
using ClubService.Infrastructure.DbContexts;
using ClubService.Infrastructure.EventHandling;
using ClubService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<EventStoreDbContext>(options =>
{
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("event-store-connection"));
});
builder.Services.AddDbContext<ReadStoreDbContext>(options =>
{
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("read-store-connection"));
});

// Repositories
builder.Services.AddScoped<IEventRepository, PostgresEventRepository>();
builder.Services.AddScoped<ISubscriptionTierReadModelRepository, SubscriptionTierReadModelRepository>();
builder.Services.AddScoped<ITennisClubReadModelRepository, TennisClubReadModelRepository>();
builder.Services.AddScoped<IAdminReadModelRepository, AdminReadModelRepository>();
builder.Services.AddScoped<IMemberReadModelRepository, MemberReadModelRepository>();

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

// Transaction
builder.Services.AddScoped<IReadStoreTransactionManager, ReadStoreTransactionManager>();

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


var chainEventHandler = new ChainEventHandler();

//TODO: Logging/errorhandling?
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
        new RedisEventReader(new CancellationTokenSource().Token, chainEventHandler, redisHost, redisStreamName,
            redisGroupName));
    
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
}

// TransactionManager
var readStoreTransactionManager = services.GetRequiredService<IReadStoreTransactionManager>();

// Read Model Repositories
var subscriptionTierReadModelRepository = services.GetRequiredService<ISubscriptionTierReadModelRepository>();
var tennisClubReadModelRepository = services.GetRequiredService<ITennisClubReadModelRepository>();
var adminReadModelRepository = services.GetRequiredService<IAdminReadModelRepository>();
var memberReadModelRepository = services.GetRequiredService<IMemberReadModelRepository>();

// Event Handlers
var subscriptionTierCreatedEventHandler = new SubscriptionTierCreatedEventHandler(subscriptionTierReadModelRepository);
// tennis club
var tennisClubRegisteredEventHandler = new TennisClubRegisteredEventHandler(tennisClubReadModelRepository);
var tennisClubLockedEventHandler = new TennisClubLockedEventHandler(tennisClubReadModelRepository);
var tennisClubNameChangedEventHandler = new TennisClubNameChangedEventHandler(tennisClubReadModelRepository);
var tennisClubSubscriptionTierChangedEventHandler =
    new TennisClubSubscriptionTierChangedEventHandler(tennisClubReadModelRepository);
var tennisClubDeletedEventHandler = new TennisClubDeletedEventHandler(tennisClubReadModelRepository);
var tennisClubUnlockedEventHandler = new TennisClubUnlockedEventHandler(tennisClubReadModelRepository);
// admin
var adminRegisteredEventHandler = new AdminRegisteredEventHandler(adminReadModelRepository);
var adminDeletedEventHandler = new AdminDeletedEventHandler(adminReadModelRepository);
var adminFullNameChangedEventHandler = new AdminFullNameChangedEventHandler(adminReadModelRepository);
// member
var memberRegisteredEventHandler = new MemberRegisteredEventHandler(memberReadModelRepository,
    tennisClubReadModelRepository, readStoreTransactionManager);
var memberLockedEventHandler = new MemberLockedEventHandler(memberReadModelRepository);
var memberUnlockedEventHandler = new MemberUnlockedEventHandler(memberReadModelRepository);
var memberDeletedEventHandler = new MemberDeletedEventHandler(memberReadModelRepository, tennisClubReadModelRepository,
    readStoreTransactionManager);
var memberFullNameChangedEventHandler = new MemberFullNameChangedEventHandler(memberReadModelRepository);

// Registration of Event Handlers
chainEventHandler.RegisterEventHandler(subscriptionTierCreatedEventHandler);
// tennis club
chainEventHandler.RegisterEventHandler(tennisClubRegisteredEventHandler);
chainEventHandler.RegisterEventHandler(tennisClubLockedEventHandler);
chainEventHandler.RegisterEventHandler(tennisClubNameChangedEventHandler);
chainEventHandler.RegisterEventHandler(tennisClubSubscriptionTierChangedEventHandler);
chainEventHandler.RegisterEventHandler(tennisClubDeletedEventHandler);
chainEventHandler.RegisterEventHandler(tennisClubUnlockedEventHandler);
// admin
chainEventHandler.RegisterEventHandler(adminRegisteredEventHandler);
chainEventHandler.RegisterEventHandler(adminDeletedEventHandler);
chainEventHandler.RegisterEventHandler(adminFullNameChangedEventHandler);
// member
chainEventHandler.RegisterEventHandler(memberRegisteredEventHandler);
chainEventHandler.RegisterEventHandler(memberLockedEventHandler);
chainEventHandler.RegisterEventHandler(memberUnlockedEventHandler);
chainEventHandler.RegisterEventHandler(memberDeletedEventHandler);
chainEventHandler.RegisterEventHandler(memberFullNameChangedEventHandler);

app.MapControllers();
app.UseExceptionHandler();
app.Run();

// For integration tests
public abstract partial class Program
{
}