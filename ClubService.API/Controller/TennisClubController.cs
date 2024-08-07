using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/tennisClubs")]
[ApiController]
[ApiVersion("1.0")]
public class TennisClubController(
    IRegisterTennisClubService registerTennisClubService,
    IUpdateTennisClubService updateTennisClubService,
    IDeleteTennisClubService deleteTennisClubService,
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    IAdminReadModelRepository adminReadModelRepository,
    IMemberReadModelRepository memberReadModelRepository
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "SYSTEM_OPERATOR")]
    public async Task<ActionResult<List<TennisClubReadModel>>> GetAllTennisClubs(
        [FromQuery] int pageSize = 0,
        int pageNumber = 1)
    {
        var tennisClubs = await tennisClubReadModelRepository.GetAllTennisClubs(pageSize, pageNumber);

        var metadata = new
        {
            pageSize,
            pageNumber
        };
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        return Ok(tennisClubs);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "SYSTEM_OPERATOR,ADMIN,MEMBER")]
    public async Task<ActionResult<TennisClubReadModel>> GetTennisClubById(Guid id)
    {
        var jwtRole = User.Claims.FirstOrDefault(c => c.Type == "groups")?.Value;
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        if (jwtRole != "SYSTEM_OPERATOR" && jwtUserTennisClubId != id.ToString())
        {
            return Unauthorized("You do not have access to this resource.");
        }

        var tennisClub = await tennisClubReadModelRepository.GetTennisClubById(id);

        if (tennisClub == null)
        {
            return NotFound($"Tennis Club with id {id} not found!");
        }

        return Ok(tennisClub);
    }

    [HttpGet("{id:guid}/admins")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "SYSTEM_OPERATOR,ADMIN")]
    public async Task<ActionResult<List<AdminReadModel>>> GetAdminsByTennisClubId(Guid id)
    {
        var jwtRole = User.Claims.FirstOrDefault(c => c.Type == "groups")?.Value;
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        if (jwtRole != "SYSTEM_OPERATOR" && jwtUserTennisClubId != id.ToString())
        {
            return Unauthorized("You do not have access to this resource.");
        }

        var admins = await adminReadModelRepository.GetAdminsByTennisClubId(id);
        return Ok(admins);
    }

    [HttpGet("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<List<MemberReadModel>>> GetMembersByTennisClubId(Guid id)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        if (jwtUserTennisClubId != id.ToString())
        {
            return Unauthorized("You do not have access to this resource.");
        }

        var members = await memberReadModelRepository.GetMembersByTennisClubId(id);
        return Ok(members);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    public async Task<ActionResult<Guid>> RegisterTennisClub(
        [FromBody] TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        var registeredTennisClubId = await registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);
        return CreatedAtAction(nameof(GetTennisClubById), new { id = registeredTennisClubId }, registeredTennisClubId);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("application/json")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> UpdateTennisClub(
        Guid id,
        [FromBody] TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        var updatedTennisClubId =
            await updateTennisClubService.UpdateTennisClub(id, tennisClubUpdateCommand, jwtUserTennisClubId);
        return Ok(updatedTennisClubId);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "SYSTEM_OPERATOR,ADMIN")]
    public async Task<ActionResult<Guid>> DeleteTennisClub(Guid id)
    {
        var jwtRole = User.Claims.FirstOrDefault(c => c.Type == "groups")?.Value;
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        var deletedTennisClubId = await deleteTennisClubService.DeleteTennisClub(id, jwtRole, jwtUserTennisClubId);
        return Ok(deletedTennisClubId);
    }

    [HttpPost("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "SYSTEM_OPERATOR")]
    public async Task<ActionResult<Guid>> LockTennisClub(Guid id)
    {
        var lockedTennisClubId = await updateTennisClubService.LockTennisClub(id);
        return Ok(lockedTennisClubId);
    }

    [HttpDelete("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "SYSTEM_OPERATOR")]
    public async Task<ActionResult<Guid>> UnlockTennisClub(Guid id)
    {
        var unlockedTennisClubId = await updateTennisClubService.UnlockTennisClub(id);
        return Ok(unlockedTennisClubId);
    }
}