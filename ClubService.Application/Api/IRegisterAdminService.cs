using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IRegisterAdminService
{
    Task<string> RegisterAdmin(AdminRegisterCommand adminRegisterCommand);
}