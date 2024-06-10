using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/login")]
[ApiController]
[ApiVersion("1.0")]
public class LoginController(ILoginService loginService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserInformationDto>> Login(
        [FromBody] LoginDto loginDto)
    {
        var userInformationDto = await loginService.Login(loginDto);
        return Ok(userInformationDto);
    }
}