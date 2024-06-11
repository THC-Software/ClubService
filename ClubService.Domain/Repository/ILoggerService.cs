namespace ClubService.Domain.Repository;

public interface ILoggerService<T>
{
    void LogDeleteAdmin(Guid id);
    void LogAdminNotFound(Guid id);
    void LogAdminDeleted(Guid id);
    void LogDeleteMember(Guid id);
    void LogMemberNotFound(Guid id);
    void LogMemberDeleted(Guid id);
    void LogInvalidOperationException(InvalidOperationException ex);
}