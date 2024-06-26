using ClubService.API.ApplicationConfigurations;
using ClubService.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddAuthenticationConfigurations(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDevelopment"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClubServiceV1"); });

    var eventStoreDbContext = services.GetRequiredService<EventStoreDbContext>();
    await eventStoreDbContext.ClearDatabase();
    await eventStoreDbContext.Database.EnsureCreatedAsync();
    await eventStoreDbContext.SeedData();

    var readStoreDbContext = services.GetRequiredService<ReadStoreDbContext>();
    await readStoreDbContext.Database.EnsureDeletedAsync();
    await readStoreDbContext.Database.EnsureCreatedAsync();

    var loginStoreDbContext = services.GetRequiredService<LoginStoreDbContext>();
    await loginStoreDbContext.Database.EnsureDeletedAsync();
    await loginStoreDbContext.Database.EnsureCreatedAsync();
}

app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();

// For integration tests
public abstract partial class Program
{
}