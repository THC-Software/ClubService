using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/members")]
[ApiController]
[ApiVersion("1.0")]
public class MemberController(
    IRegisterMemberService registerMemberService,
    IUpdateMemberService updateMemberService,
    IDeleteMemberService deleteMemberService,
    IMemberReadModelRepository memberReadModelRepository)
    : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN,MEMBER")]
    public async Task<ActionResult<MemberReadModel>> GetMemberById(Guid id)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        if (jwtUserTennisClubId == null)
        {
            return Unauthorized("Authentication error.");
        }

        var memberReadModel = await memberReadModelRepository.GetMemberById(id);
        if (memberReadModel == null)
        {
            return NotFound($"Member with id '{id}' not found!");
        }

        if (!jwtUserTennisClubId.Equals(memberReadModel.TennisClubId.Id.ToString()))
        {
            return Forbid("You do not have access to this resource.");
        }

        return Ok(memberReadModel);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> RegisterMember([FromBody] MemberRegisterCommand memberRegisterCommand)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;

        var registeredMemberId =
            await registerMemberService.RegisterMember(memberRegisterCommand, jwtUserTennisClubId);

        return CreatedAtAction(nameof(GetMemberById), new { id = registeredMemberId }, registeredMemberId);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "MEMBER")]
    public async Task<ActionResult<Guid>> UpdateMember(Guid id, MemberUpdateCommand memberUpdateCommand)
    {
        var jwtUserId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;

        var updatedMemberId =
            await updateMemberService.UpdateMember(id, memberUpdateCommand, jwtUserId, jwtUserTennisClubId);

        return Ok(updatedMemberId);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> DeleteMember(Guid id)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        var deletedMemberId = await deleteMemberService.DeleteMember(id, jwtUserTennisClubId);
        return Ok(deletedMemberId);
    }

    [HttpPost("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> LockMember(Guid id)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        var lockedMemberId = await updateMemberService.LockMember(id, jwtUserTennisClubId);
        return Ok(lockedMemberId);
    }

    [HttpDelete("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<Guid>> UnlockMember(Guid id)
    {
        var jwtUserTennisClubId = User.Claims.FirstOrDefault(c => c.Type == "tennisClubId")?.Value;
        var unlockedMemberId = await updateMemberService.UnlockMember(id, jwtUserTennisClubId);
        return Ok(unlockedMemberId);
    }
}