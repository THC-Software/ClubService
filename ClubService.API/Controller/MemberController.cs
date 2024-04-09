using ClubService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v1/members")]
[ApiController]
public class MemberController : ControllerBase
{
    [HttpGet("/{memberId}")]
    [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MemberDto>> GetMemberById(string memberId)
    {
        return await Task.FromResult(
            new MemberDto("", "", "", "", false));
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateMember(MemberCreateUpdateDto memberCreateUpdateDto)
    {
        return await Task.FromResult("");
    }
    
    [HttpPut]
    public async Task<ActionResult<string>> UpdateMember(MemberCreateUpdateDto memberCreateUpdateDto)
    {
        return await Task.FromResult("");
    }
    
    [HttpDelete("/{memberId}")]
    public async Task<ActionResult<string>> DeleteMember(string memberId)
    {
        return await Task.FromResult("");
    }
    
    [HttpPost("/{memberId}/lock")]
    public async Task<IActionResult> LockMember(string memberId)
    {
        return await Task.FromResult(Ok());
    }
    
    [HttpPost("/{memberId}/unlock")]
    public async Task<IActionResult> UnlockMember(string memberId)
    {
        return await Task.FromResult(Ok());
    }
}