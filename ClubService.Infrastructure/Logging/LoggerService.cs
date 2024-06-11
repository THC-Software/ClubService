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

    public void LogRegisterAdmin(string username, string firstName, string lastName, Guid tennisClubId)
    {
        logger.LogInformation(
            "RegisterAdmin called with username '{username}', firstName '{firstName}', lastName '{lastName}' " +
            "and tennisClubId '{tennisClubId}''.", username, firstName, lastName, tennisClubId);
    }

    public void LogAdminUsernameAlreadyExists(string username, string tennisClubName, Guid tennisClubId)
    {
        logger.LogError(
            "Admin username '{username}' already exists in tennis club '{tennisClubName}' ({tennisClubId}).", username,
            tennisClubName, tennisClubId);
    }

    public void LogAdminRegistered(Guid id)
    {
        logger.LogInformation("Registered admin with id '{id}'.", id);
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

    public void LogRegisterMember(string firstName, string lastName, string email, Guid tennisClubId)
    {
        logger.LogInformation(
            "RegisterMember called with email '{email}', firstName '{firstName}', lastName '{lastName}' " +
            "and tennisClubId '{tennisClubId}''.", email, firstName, lastName, tennisClubId);
    }

    public void LogMemberLimitExceeded(Guid tennisClubId, int maxMemberCount)
    {
        logger.LogError("Member limit of {maxMemberCount} exceeded for tennis club with id '{id}'.", maxMemberCount,
            tennisClubId);
    }

    public void LogMemberEmailAlreadyExists(string email, string tennisClubName, Guid tennisClubId)
    {
        logger.LogError(
            "Member email '{email}' already exists in tennis club '{tennisClubName}' ({tennisClubId}).", email,
            tennisClubName, tennisClubId);
    }

    public void LogMemberRegistered(Guid id)
    {
        logger.LogInformation("Registered member with id '{id}'.", id);
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