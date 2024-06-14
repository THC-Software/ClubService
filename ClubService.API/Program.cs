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
using ClubService.Infrastructure.Configurations;
using ClubService.Infrastructure.DbContexts;
using ClubService.Infrastructure.EventHandling;
using ClubService.Infrastructure.Logging;
using ClubService.Infrastructure.Mail;
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("login-store-connection"));
});

// Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});
builder.Services.AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>));

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
builder.Services.AddScoped<ChainEventHandler>();

// Mail
builder.Services.Configure<SmtpConfiguration>(builder.Configuration.GetSection("SmtpConfiguration"));
builder.Services.AddScoped<IMailService, MailService>();

// Redis
builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("RedisConfiguration"));
builder.Services.AddHostedService<RedisEventReader>();

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

app.MapControllers();
app.UseExceptionHandler();
app.Run();

// For integration tests
public abstract partial class Program
{
}