using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/users")]
[ApiController]
[ApiVersionNeutral]
public class UserController(ILoginService loginService, IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<ActionResult<UserInformationDto>> Login(
        [FromBody] LoginCommand loginCommand)
    {
        var userInformationDto = await loginService.Login(loginCommand);
        return Ok(userInformationDto);
    }

    [HttpPatch("changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    [Authorize(Roles = "SYSTEM_OPERATOR,ADMIN,MEMBER")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand changePasswordCommand)
    {
        var jwtUserId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        await userService.ChangePassword(changePasswordCommand, jwtUserId);
        return Ok();
    }
}