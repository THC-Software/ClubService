namespace ClubService.Application.Api.Exceptions;

public interface IDeleteAdminService
{
    Task<string> DeleteAdmin(string id);
}