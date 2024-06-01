namespace ClubService.Application.Api;

public interface IUpdateAdminService
{
    Task<string> ChangeFullName(string id, string firstName, string lastName);
}