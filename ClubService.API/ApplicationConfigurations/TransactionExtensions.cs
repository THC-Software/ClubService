using ClubService.Domain.Repository.Transaction;
using ClubService.Infrastructure;
using ClubService.Infrastructure.DbContexts;

namespace ClubService.API.ApplicationConfigurations;

public static class TransactionExtensions
{
    public static void AddTransactionConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IReadStoreTransactionManager, TransactionManager<ReadStoreDbContext>>();
        services.AddScoped<IEventStoreTransactionManager, TransactionManager<EventStoreDbContext>>();
    }
}