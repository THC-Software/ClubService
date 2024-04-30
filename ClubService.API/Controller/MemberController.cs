using Asp.Versioning;
using ClubService.Application.Commands;
using ClubService.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/members")]
[ApiController]
[ApiVersion("1.0")]
public class MemberController : ControllerBase
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
    public async Task<ActionResult<string>> CreateMember(MemberRegisterCommand memberRegisterCommand)
    {
        return await Task.FromResult("");
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
    
    [HttpPost("{memberId}/lock")]
    public async Task<ActionResult<string>> LockMember(string memberId)
    {
        return await Task.FromResult(Ok());
    }
    
    [HttpDelete("{memberId}/lock")]
    public async Task<ActionResult<string>> UnlockMember(string memberId)
    {
        return await Task.FromResult(Ok());
    }
}