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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> RegisterTennisClub(
        [FromBody] TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        var registeredTennisClubId = await registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);
        return CreatedAtAction(nameof(RegisterTennisClub), new { id = registeredTennisClubId }, registeredTennisClubId);
    }
    
    [HttpPatch("{clubId}")]
    public async Task<ActionResult<string>> UpdateTennisClub(
        string clubId,
        [FromBody] TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        if (tennisClubUpdateCommand.SubscriptionTierId != null)
        {
            var updatedTennisClubId =
                await updateTennisClubService.ChangeSubscriptionTier(clubId,
                    tennisClubUpdateCommand.SubscriptionTierId);
            return Ok(updatedTennisClubId);
        }
        
        if (tennisClubUpdateCommand.Name != null)
        {
            // TODO: Implement
            throw new NotImplementedException("Currently it is not possible to update the name of a tennis club.");
        }
        
        return BadRequest("You have to provide either a name or a subscription tier!");
    }
    
    [HttpPost("{clubId}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> LockTennisClub(string clubId)
    {
        var lockedTennisClubId = await updateTennisClubService.LockTennisClub(clubId);
        return Ok(lockedTennisClubId);
    }
    
    [HttpDelete("{clubId}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UnlockTennisClub(string clubId)
    {
        var unlockedTennisClubId = await updateTennisClubService.UnlockTennisClub(clubId);
        return Ok(unlockedTennisClubId);
    }
}