namespace ClubService.Application.Api;

public interface IUpdateAdminService
{
    Task<Guid> ChangeFullName(Guid id, string firstName, string lastName);
}