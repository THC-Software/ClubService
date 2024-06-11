namespace ClubService.Domain.Repository;

public interface ILoggerService<T>
{
    public void LogDeleteAdmin(Guid id);
    public void LogAdminNotFound(Guid id);
    public void LogAdminDeleted(Guid id);

    public void LogInvalidOperationException(InvalidOperationException ex);
}