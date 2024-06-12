using ClubService.Domain.Event;

namespace ClubService.Domain.Repository;

public interface ILoggerService<T>
{
    void LogDeleteAdmin(Guid id);
    void LogAdminNotFound(Guid id);
    void LogAdminDeleted(Guid id);
    void LogRegisterAdmin(string username, string firstName, string lastName, Guid tennisClubId);
    void LogAdminUsernameAlreadyExists(string username, string tennisClubName, Guid tennisClubId);
    void LogAdminRegistered(Guid id);
    void LogAdminChangeFullName(Guid id, string firstName, string lastName);
    void LogAdminFullNameChanged(Guid id);
    void LogAdminDeletedEventHandler(DomainEnvelope<IDomainEvent> domainEnvelope);
    void LogAdminFullNameChangedEventHandler(DomainEnvelope<IDomainEvent> domainEnvelope);
    void LogAdminRegisteredEventHandler(DomainEnvelope<IDomainEvent> domainEnvelope);
    void LogDeleteMember(Guid id);
    void LogMemberNotFound(Guid id);
    void LogMemberDeleted(Guid id);
    void LogRegisterMember(string firstName, string lastName, string email, Guid tennisClubId);
    void LogMemberLimitExceeded(Guid tennisClubId, int maxMemberCount);
    void LogMemberEmailAlreadyExists(string email, string tennisClubName, Guid tennisClubId);
    void LogMemberRegistered(Guid id);
    void LogLockMember(Guid id);
    void LogMemberLocked(Guid id);
    void LogUnlockMember(Guid id);
    void LogMemberUnlocked(Guid id);
    void LogMemberChangeFullName(Guid id, string firstName, string lastName);
    void LogMemberFullNameChanged(Guid id);
    void LogMemberChangeEmail(Guid id, string email);
    void LogMemberEmailChanged(Guid id);
    void LogDeleteTennisClub(Guid id);
    void LogTennisClubNotFound(Guid id);
    void LogTennisClubDeleted(Guid id);
    void LogRegisterTennisClub(string name, Guid subscriptionTierId);
    void LogTennisClubRegistered(Guid id);
    void LogLockTennisClub(Guid id);
    void LogTennisClubLocked(Guid id);
    void LogUnlockTennisClub(Guid id);
    void LogTennisClubUnlocked(Guid id);
    void LogUpdateTennisClub(Guid id, string? name, Guid? subscriptionTierId);
    void LogTennisClubUpdated(Guid id);
    void LogSubscriptionTierNotFound(Guid id);
    void LogLogin(string username, Guid tennisClubId);
    void LogUserNotFound(Guid id);
    void LogUserNotFound(string username);
    void LogLoginFailed(Guid id);
    void LogUserLoggedIn(Guid id, string username, string userRole, string status);
    void LogInvalidOperationException(InvalidOperationException ex);
    void LogValidationFailure(string validationMessage);
}