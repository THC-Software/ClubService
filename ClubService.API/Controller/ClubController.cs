using ClubService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v1/clubs")]
[ApiController]
public class ClubController : ControllerBase
{
    [HttpGet("{clubId}/members")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembersByClub(string clubId)
    {
        var memberDtos = new List<MemberDto>();
        return await Task.FromResult(memberDtos);
    }
    
    [HttpPost]
    public async Task<ActionResult<string>> CreateTennisClub([FromBody] ClubCreateUpdateDto clubCreateUpdateDto)
    {
        return await Task.FromResult(Ok());
    }
    
    [HttpPut("{clubId}")]
    public async Task<ActionResult<string>> UpdateTennisClub(string clubId, [FromBody] ClubCreateUpdateDto clubCreateUpdateDto)
    {
        return await Task.FromResult(Ok());
    }
}
