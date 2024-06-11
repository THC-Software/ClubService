using ClubService.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace ClubService.Infrastructure.Logging;

public class LoggerService<T>(ILogger<T> logger) : ILoggerService<T>
{
    public void LogDeleteAdmin(Guid id)
    {
        logger.LogInformation("DeleteAdmin called with id '{id}'.", id);
    }

    public void LogAdminNotFound(Guid id)
    {
        logger.LogError("Admin with id '{id}' not found.", id);
    }

    public void LogAdminDeleted(Guid id)
    {
        logger.LogInformation("Deleted admin with id '{id}'.", id);
    }

    public void LogDeleteMember(Guid id)
    {
        logger.LogInformation("DeleteMember called with id '{id}'.", id);
    }

    public void LogMemberNotFound(Guid id)
    {
        logger.LogError("Member with id '{id}' not found.", id);
    }

    public void LogMemberDeleted(Guid id)
    {
        logger.LogInformation("Deleted member with id '{id}'.", id);
    }

    public void LogDeleteTennisClub(Guid id)
    {
        logger.LogInformation("DeleteTennisClub called with id '{id}'.", id);
    }

    public void LogTennisClubNotFound(Guid id)
    {
        logger.LogError("Tennis Club with id '{id}' not found.", id);
    }

    public void LogTennisClubDeleted(Guid id)
    {
        logger.LogInformation("Deleted tennis club with id '{id}'.", id);
    }

    public void LogRegisterTennisClub(string name, Guid subscriptionTierId)
    {
        logger.LogInformation("RegisterTennisClub called with name '{name}' and subscription tier id '{id}'.", name,
            subscriptionTierId);
    }

    public void LogTennisClubRegistered(Guid id)
    {
        logger.LogInformation("Registered tennis club with id '{id}'.", id);
    }

    public void LogSubscriptionTierNotFound(Guid id)
    {
        logger.LogError("Subscription tier with id '{id}' not found.", id);
    }

    public void LogInvalidOperationException(InvalidOperationException ex)
    {
        logger.LogError("Error: {error}", ex.Message);
    }
}