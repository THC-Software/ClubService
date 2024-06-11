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
    void LogDeleteMember(Guid id);
    void LogMemberNotFound(Guid id);
    void LogMemberDeleted(Guid id);
    void LogRegisterMember(string firstName, string lastName, string email, Guid tennisClubId);
    void LogMemberLimitExceeded(Guid tennisClubId, int maxMemberCount);
    void LogMemberEmailAlreadyExists(string email, string tennisClubName, Guid tennisClubId);
    void LogMemberRegistered(Guid id);
    void LogDeleteTennisClub(Guid id);
    void LogTennisClubNotFound(Guid id);
    void LogTennisClubDeleted(Guid id);
    void LogRegisterTennisClub(string name, Guid subscriptionTierId);
    void LogTennisClubRegistered(Guid id);
    void LogSubscriptionTierNotFound(Guid id);
    void LogInvalidOperationException(InvalidOperationException ex);
}