using System.Text.Json;
using ClubService.Domain.Event;
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

    public void LogAdminChangeFullName(Guid id, string firstName, string lastName)
    {
        logger.LogInformation("ChangeFullName called with id '{id}', firstName '{firstName} and lastName '{lastName}'.",
            id, firstName, lastName);
    }

    public void LogAdminFullNameChanged(Guid id)
    {
        logger.LogInformation("Changed full name for admin with id '{id}'.", id);
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

    public void LogLockMember(Guid id)
    {
        logger.LogInformation("LockMember called with id '{id}'.", id);
    }

    public void LogMemberLocked(Guid id)
    {
        logger.LogInformation("Locked member with id '{id}'.", id);
    }

    public void LogUnlockMember(Guid id)
    {
        logger.LogInformation("UnlockMember called with id '{id}'.", id);
    }

    public void LogMemberUnlocked(Guid id)
    {
        logger.LogInformation("Unlocked member with id '{id}'.", id);
    }

    public void LogUpdateMember(Guid id, string? firstName, string? lastName, string? email)
    {
        logger.LogInformation(
            "LogUpdateMember called with id '{id}', firstName '{firstName}', lastName '{lastName}' and email '{email}'.",
            id, firstName,
            lastName, email);
    }

    public void LogMemberUpdated(Guid id)
    {
        logger.LogInformation("Updated member with id '{id}'.", id);
    }

    public void LogMemberFullNameChanged(Guid id)
    {
        logger.LogInformation("Changed full name for member with id '{id}'.", id);
    }

    public void LogMemberEmailChanged(Guid id)
    {
        logger.LogInformation("Changed email for member with id '{id}'.", id);
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

    public void LogLockTennisClub(Guid id)
    {
        logger.LogInformation("LockTennisClub called with id '{id}'.", id);
    }

    public void LogTennisClubLocked(Guid id)
    {
        logger.LogInformation("Locked tennis club with id '{id}'.", id);
    }

    public void LogUnlockTennisClub(Guid id)
    {
        logger.LogInformation("UnlockTennisClub called with id '{id}'.", id);
    }

    public void LogTennisClubUnlocked(Guid id)
    {
        logger.LogInformation("Unlocked tennis club with id '{id}'.", id);
    }

    public void LogUpdateTennisClub(Guid id, string? name, Guid? subscriptionTierId)
    {
        logger.LogInformation(
            "UpdateTennisClub called with id '{id}', name '{name}' and subscription tier id '{subscriptionTierId}'.",
            id, name, subscriptionTierId);
    }

    public void LogTennisClubUpdated(Guid id)
    {
        logger.LogInformation("Updated tennis club with id '{id}'.", id);
    }

    public void LogTennisClubNameChanged(Guid id)
    {
        logger.LogInformation("Changed name for tennis club with id '{id}'.", id);
    }

    public void LogTennisClubSubscriptionTierChanged(Guid id)
    {
        logger.LogInformation("Changed subscription tier for tennis club with id '{id}'.", id);
    }

    public void LogSubscriptionTierCreated(Guid id)
    {
        logger.LogInformation("Created subscription tier with id '{id}'.", id);
    }

    public void LogSubscriptionTierNotFound(Guid id)
    {
        logger.LogError("Subscription tier with id '{id}' not found.", id);
    }

    public void LogLogin(string username, Guid tennisClubId)
    {
        logger.LogInformation("Login called with username '{username} and tennis club id '{tennisClubId}'.", username,
            tennisClubId);
    }

    public void LogLogin(string username)
    {
        logger.LogInformation("Login called for system operator with username '{username}'.", username);
    }

    public void LogUserNotFound(Guid id)
    {
        logger.LogError("User with id '{id}' not found.", id);
    }

    public void LogUserNotFound(string username)
    {
        logger.LogError("User with username '{username}' not found.", username);
    }

    public void LogLoginFailed(Guid id)
    {
        logger.LogError("Login failed for user with id '{id}'.", id);
    }

    public void LogUserLoggedIn(Guid id, string username, string userRole, string status)
    {
        logger.LogInformation(
            "User logged in with id '{id}', username '{username}', user role '{userRole}' and status '{status}'", id,
            username, userRole, status);
    }

    public void LogTournamentConfirmed(Guid id)
    {
        logger.LogInformation("Tournament confirmed with id '{id}'.", id);
    }

    public void LogTournamentNotFound(Guid id)
    {
        logger.LogError("Tournament with id '{id}' not found.", id);
    }

    public void LogTournamentCanceled(Guid id)
    {
        logger.LogInformation("Tournament canceled with id '{id}'.", id);
    }

    public void LogInvalidOperationException(InvalidOperationException ex)
    {
        logger.LogError("Error: {error}", ex.Message);
    }

    public void LogValidationFailure(string validationMessage)
    {
        logger.LogError("Validation Error: {validationMessage}", validationMessage);
    }

    public void LogHandleEvent(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        logger.LogInformation("Handling event: {domainEnvelope}", domainEnvelope);
    }

    public void LogRejectEvent(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        logger.LogDebug("Rejected event: {domainEnvelope}", domainEnvelope);
    }

    public void LogEventReaderStart()
    {
        logger.LogInformation("EventReader is running. ");
    }

    public void LogEventReaderStop()
    {
        logger.LogInformation("EventReader is stopping.");
    }

    public void LogException(Exception exception)
    {
        logger.LogError(exception, "Exception occured:");
    }

    public void LogEmptyStreamEntry()
    {
        logger.LogWarning("Received empty or null JSON value");
    }

    public void LogJsonException(JsonException jsonException, string jsonValue)
    {
        logger.LogError(jsonException, "JSON parsing error: Invalid JSON format. Value: {Json}", jsonValue);
    }

    public void LogJsonMissingProperties(string jsonValue)
    {
        logger.LogWarning("Message missing 'payload' or 'after' properties or 'after' is null: {Json}", jsonValue);
    }

    public void LogEmailMessageRelayStop()
    {
        logger.LogInformation("EmailMessageRelay is stopping.");
    }

    public void LogSystemOperatorRegistered(Guid id)
    {
        logger.LogInformation("Registered system operator with id '{id}'.", id);
    }

    public void LogInvalidEMailAddress(string emailAddress)
    {
        logger.LogError("The following email address is invalid: {emailMessageRecipientEMailAddress}", emailAddress);
    }
    
    public void LogDuplicateSeedData()
    {
        logger.LogInformation("Seed data already seeded");
    }
}