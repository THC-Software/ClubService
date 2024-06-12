using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MemberReadModel>> GetMemberById(Guid id)
    {
        var memberReadModel = await memberReadModelRepository.GetMemberById(id);
        
        if (memberReadModel == null)
        {
            return NotFound($"Member with id {id} not found!");
        }
        
        return Ok(memberReadModel);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> RegisterMember([FromBody] MemberRegisterCommand memberRegisterCommand)
    {
        var registeredMemberId = await registerMemberService.RegisterMember(memberRegisterCommand);
        return CreatedAtAction(nameof(RegisterMember), new { id = registeredMemberId }, registeredMemberId);
    }
    
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> UpdateMember(Guid id, MemberUpdateCommand memberUpdateCommand)
    {
        var updatedMemberId = await updateMemberService.UpdateMember(id, memberUpdateCommand);
        return Ok(updatedMemberId);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> DeleteMember(Guid id)
    {
        var deletedMemberId = await deleteMemberService.DeleteMember(id);
        return Ok(deletedMemberId);
    }
    
    [HttpPost("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> LockMember(Guid id)
    {
        var lockedMemberId = await updateMemberService.LockMember(id);
        return Ok(lockedMemberId);
    }
    
    [HttpDelete("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> UnlockMember(Guid id)
    {
        var unlockedMemberId = await updateMemberService.UnlockMember(id);
        return Ok(unlockedMemberId);
    }
}