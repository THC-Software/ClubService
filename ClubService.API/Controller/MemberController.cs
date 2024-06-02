using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.ReadModel;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/members")]
[ApiController]
[ApiVersion("1.0")]
public class MemberController(
    IRegisterMemberService registerMemberService,
    IUpdateMemberService updateMemberService,
    IDeleteMemberService deleteMemberService)
    : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MemberReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberReadModel>> GetMemberById(string id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> RegisterMember([FromBody] MemberRegisterCommand memberRegisterCommand)
    {
        var registeredMemberId = await registerMemberService.RegisterMember(memberRegisterCommand);
        return CreatedAtAction(nameof(RegisterMember), new { id = registeredMemberId }, registeredMemberId);
    }
    
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UpdateMember(string id, MemberUpdateCommand memberUpdateCommand)
    {
        if (memberUpdateCommand.FirstName != null && memberUpdateCommand.LastName != null)
        {
            var updatedMemberId = await updateMemberService.ChangeFullName(
                id,
                memberUpdateCommand.FirstName,
                memberUpdateCommand.LastName
            );
            return Ok(updatedMemberId);
        }
        
        if (memberUpdateCommand.Email != null)
        {
            throw new NotImplementedException();
        }
        
        return BadRequest("You have to provide either first and last name or an e-mail address!");
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> DeleteMember(string id)
    {
        var deletedMemberId = await deleteMemberService.DeleteMember(id);
        return Ok(deletedMemberId);
    }
    
    [HttpPost("{id}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> LockMember(string id)
    {
        var lockedMemberId = await updateMemberService.LockMember(id);
        return Ok(lockedMemberId);
    }
    
    [HttpDelete("{id}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UnlockMember(string id)
    {
        var unlockedMemberId = await updateMemberService.UnlockMember(id);
        return Ok(unlockedMemberId);
    }
}