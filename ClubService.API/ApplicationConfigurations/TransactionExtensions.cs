using ClubService.Domain.Repository.Transaction;
using ClubService.Infrastructure;

namespace ClubService.API.ApplicationConfigurations;

public static class TransactionExtensions
{
    public static void AddTransactionConfigurations(this IServiceCollection services)
    {
        services.AddScoped<ITransactionManager, TransactionManager>();
    }
}