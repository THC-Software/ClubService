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
    [ProducesResponseType<string>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterTennisClub(
        [FromBody] TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        var registeredTennisClubId = await registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);
        return CreatedAtAction(nameof(RegisterTennisClub), new { id = registeredTennisClubId }, registeredTennisClubId);
    }
    
    [HttpPut("{clubId}")]
    public async Task<ActionResult<string>> UpdateTennisClub(
        string clubId,
        [FromBody] TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        return await Task.FromResult(Ok());
    }
    
    [HttpPost("{clubId}/lock")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> LockTennisClub(string clubId)
    {
        var lockedTennisClubId = await updateTennisClubService.LockTennisClub(clubId);
        return Ok(lockedTennisClubId);
    }
    
    [HttpDelete("{clubId}/lock")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UnlockTennisClub(string clubId)
    {
        var unlockedTennisClubId = await updateTennisClubService.UnlockTennisClub(clubId);
        return Ok(unlockedTennisClubId);
    }
}