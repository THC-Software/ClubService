using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IRegisterAdminService
{
    Task<Guid> RegisterAdmin(AdminRegisterCommand adminRegisterCommand);
}