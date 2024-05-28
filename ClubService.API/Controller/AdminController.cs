using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/admins")]
[ApiController]
[ApiVersion("1.0")]
public class AdminController(IRegisterAdminService registerAdminService, IDeleteAdminService deleteAdminService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> RegisterAdmin(
        [FromBody] AdminRegisterCommand adminRegisterCommand)
    {
        var registeredAdminId = await registerAdminService.RegisterAdmin(adminRegisterCommand);
        return CreatedAtAction(nameof(RegisterAdmin), new { id = registeredAdminId }, registeredAdminId);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> DeleteAdmin(string id)
    {
        var deletedAdminId = await deleteAdminService.DeleteAdmin(id);
        return Ok(deletedAdminId);
    }
}