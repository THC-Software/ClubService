using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/admins")]
[ApiController]
[ApiVersion("1.0")]
public class AdminController(
    IRegisterAdminService registerAdminService,
    IDeleteAdminService deleteAdminService,
    IUpdateAdminService updateAdminService,
    IAdminReadModelRepository adminReadModelRepository)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> RegisterAdmin(
        [FromBody] AdminRegisterCommand adminRegisterCommand)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        var registeredAdminId = await registerAdminService.RegisterAdmin(adminRegisterCommand, jwtUserTennisClubId);
        return CreatedAtAction(nameof(GetAdminById), new { id = registeredAdminId }, registeredAdminId);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> DeleteAdmin(Guid id)
    {
        var jwtUserId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        var deletedAdminId = await deleteAdminService.DeleteAdmin(id, jwtUserId, jwtUserTennisClubId);
        return Ok(deletedAdminId);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> UpdateAdmin(Guid id, [FromBody] AdminUpdateCommand adminUpdateCommand)
    {
        var jwtUserId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var updatedAdminId = await updateAdminService.ChangeFullName(
            id,
            adminUpdateCommand.FirstName,
            adminUpdateCommand.LastName,
            jwtUserId
        );
        return Ok(updatedAdminId);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "SYSTEM_OPERATOR,ADMIN")]
    public async Task<ActionResult<AdminReadModel>> GetAdminById(Guid id)
    {
        var jwtRole = User.Claims.FirstOrDefault(c => c.Type == "groups")?.Value;
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;

        if (jwtRole == null || jwtUserTennisClubId == null)
        {
            return Unauthorized("You do not have access to this resource.");
        }

        var adminReadModel = await adminReadModelRepository.GetAdminById(id);

        if (adminReadModel == null)
        {
            return NotFound($"Admin with id {id} not found!");
        }

        if (!jwtRole.Equals("SYSTEM_OPERATOR") &&
            !jwtUserTennisClubId.Equals(adminReadModel.TennisClubId.Id.ToString()))
        {
            return Unauthorized("You do not have access to this resource.");
        }

        return Ok(adminReadModel);
    }
}