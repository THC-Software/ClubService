using Asp.Versioning;
using ClubService.Application.Commands;
using ClubService.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/clubs")]
[ApiController]
[ApiVersion("1.0")]
public class ClubController : ControllerBase
{
    [HttpGet("{clubId}/members")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembersByClub(string clubId)
    {
        var memberDtos = new List<MemberDto>();
        return await Task.FromResult(memberDtos);
    }
    
    [HttpPost]
    public async Task<ActionResult<string>> CreateTennisClub([FromBody] TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        return await Task.FromResult(Ok());
    }
    
    [HttpPut("{clubId}")]
    public async Task<ActionResult<string>> UpdateTennisClub(string clubId, [FromBody] TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        return await Task.FromResult(Ok());
    }
}
