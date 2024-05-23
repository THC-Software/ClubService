using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/members")]
[ApiController]
[ApiVersion("1.0")]
public class MemberController(IRegisterMemberService registerMemberService, IUpdateMemberService updateMemberService)
    : ControllerBase
{
    [HttpGet("{memberId}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMemberById(string memberId)
    {
        return await Task.FromResult(
            new MemberDto("", "", "", "", false));
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
    
    [HttpPut("{memberId}")]
    public async Task<ActionResult<string>> UpdateMember(string memberId, MemberUpdateCommand memberUpdateCommand)
    {
        return await Task.FromResult("");
    }
    
    [HttpDelete("{memberId}")]
    public async Task<ActionResult<string>> DeleteMember(string memberId)
    {
        return await Task.FromResult("");
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