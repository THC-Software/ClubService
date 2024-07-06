using ClubService.Application.Commands;
using ClubService.Application.Dto;

namespace ClubService.Application.Api;

public interface ILoginService
{
    Task<UserInformationDto> Login(LoginCommand loginCommand);
}