using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/tennisClubs")]
[ApiController]
[ApiVersion("1.0")]
public class TennisClubController(
    IRegisterTennisClubService registerTennisClubService,
    IUpdateTennisClubService updateTennisClubService) : ControllerBase
{
    [HttpGet("{clubId}/members")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembersByClub(string clubId)
    {
        var memberDtos = new List<MemberDto>();
        return await Task.FromResult(memberDtos);
    }

    [HttpPost]
    public async Task<ActionResult<string>> RegisterTennisClub(
        [FromBody] TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        return await registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);
    }

    [HttpPut("{clubId}")]
    public async Task<ActionResult<string>> UpdateTennisClub(string clubId,
        [FromBody] TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        return await Task.FromResult(Ok());
    }

    [HttpPost("{clubId}/lock")]
    public async Task<ActionResult<string>> LockTennisClub(string clubId)
    {
        return await Task.FromResult(updateTennisClubService.LockTennisClub(clubId));
    }
}