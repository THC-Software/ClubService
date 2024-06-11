namespace ClubService.Domain.Repository;

public interface ILoggerService<T>
{
    void LogDeleteAdmin(Guid id);
    void LogAdminNotFound(Guid id);
    void LogAdminDeleted(Guid id);
    void LogDeleteMember(Guid id);
    void LogMemberNotFound(Guid id);
    void LogMemberDeleted(Guid id);
    void LogDeleteTennisClub(Guid id);
    void LogTennisClubNotFound(Guid id);
    void LogTennisClubDeleted(Guid id);
    void LogRegisterTennisClub(string name, Guid subscriptionTierId);
    void LogTennisClubRegistered(Guid id);
    void LogSubscriptionTierNotFound(Guid id);
    void LogInvalidOperationException(InvalidOperationException ex);
}