using ClubService.API;
using ClubService.API.ApplicationConfigurations;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddJsonFile("appsettings.Production.json");
    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.AddSubstitution();
}

builder.Services.AddDatabaseConfigurations(builder.Configuration);
builder.Services.AddLoggingConfigurations(builder.Configuration);
builder.Services.AddRepositoryConfigurations();
builder.Services.AddServiceConfigurations();
builder.Services.AddTransactionConfigurations();
builder.Services.AddEventHandlerConfigurations();
builder.Services.AddExternalServiceConfigurations(builder.Configuration);
builder.Services.AddApiVersioningConfigurations();
builder.Services.AddSwaggerConfigurations();
builder.Services.AddExceptionHandlerConfigurations();
builder.Services.AddControllers();

// By configuring a default authentication scheme, we ensure that the authorization middleware works correctly
// and returns the appropriate 403 Forbidden response when the user does not have the required role.
// The custom middleware JwtClaimsMiddleware.cs continues to handle the extraction of claims from the JWT, while the dummy
// authentication scheme satisfies the requirement for a default authentication handler.
builder.Services
    .AddAuthentication("BasicScheme")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicScheme", null);

builder.Services.AddAuthorization();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var eventStoreDbContext = services.GetRequiredService<EventStoreDbContext>();
var readStoreDbContext = services.GetRequiredService<ReadStoreDbContext>();
var loginStoreDbContext = services.GetRequiredService<LoginStoreDbContext>();

if (builder.Environment.IsProduction())
{
    if (eventStoreDbContext.Database.GetPendingMigrations().Any())
    {
        await eventStoreDbContext.Database.MigrateAsync();
    }

    if (readStoreDbContext.Database.GetPendingMigrations().Any())
    {
        await readStoreDbContext.Database.MigrateAsync();
    }

    if (loginStoreDbContext.Database.GetPendingMigrations().Any())
    {
        await loginStoreDbContext.Database.MigrateAsync();
    }
}
else if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDevelopment"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClubServiceV1"); });

    await eventStoreDbContext.Database.EnsureCreatedAsync();

    try
    {
        await eventStoreDbContext.SeedTestData();
    }
    catch (DbUpdateException)
    {
        var logger = services.GetRequiredService<ILoggerService<Program>>();
        logger.LogDuplicateSeedData();
    }
    
    await readStoreDbContext.Database.EnsureDeletedAsync();
    await readStoreDbContext.Database.EnsureCreatedAsync();
    
    await loginStoreDbContext.Database.EnsureDeletedAsync();
    await loginStoreDbContext.Database.EnsureCreatedAsync();
}

app.UseExceptionHandler();
app.UseMiddleware<JwtClaimsMiddleware>(); // Use custom middleware to extract JWT claims
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// For integration tests
public abstract partial class Program
{
}