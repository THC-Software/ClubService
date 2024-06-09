using ClubService.Application.Dto;

namespace ClubService.Application.Api;

public interface ILoginService
{
    Task<UserInformationDto> Login(LoginDto loginDto);
}