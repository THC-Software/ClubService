using ClubService.Domain.Repository;
using ClubService.Infrastructure.Repositories;

namespace ClubService.API.ApplicationConfigurations;

public static class RepositoryExtensions
{
    public static void AddRepositoryConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IEventRepository, PostgresEventRepository>();
        services.AddScoped<ISubscriptionTierReadModelRepository, SubscriptionTierReadModelRepository>();
        services.AddScoped<ITennisClubReadModelRepository, TennisClubReadModelRepository>();
        services.AddScoped<IAdminReadModelRepository, AdminReadModelRepository>();
        services.AddScoped<IMemberReadModelRepository, MemberReadModelRepository>();
        services.AddScoped<ITournamentReadModelRepository, TournamentReadModelRepository>();
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
        services.AddScoped<ILoginRepository, LoginRepository>();
        services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
    }
}