using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Dto;
using ClubService.Domain.Model.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}")]
[ApiController]
[ApiVersion("1.0")]
public class UserController(ILoginService loginService, IUserService userService) : ControllerBase
{
    [HttpPost("/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserInformationDto>> Login(
        [FromBody] LoginDto loginDto)
    {
        var userInformationDto = await loginService.Login(loginDto);
        return Ok(userInformationDto);
    }

    [HttpPatch("/changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        await userService.ChangePassword(changePasswordDto);
        return Ok();
    }
}