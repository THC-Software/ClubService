namespace ClubService.Application.Api;

public interface IDeleteAdminService
{
    Task<Guid> DeleteAdmin(Guid id);
}