using Asp.Versioning;
using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/tennisClubs")]
[ApiController]
[ApiVersion("1.0")]
public class TennisClubController(
    IRegisterTennisClubService registerTennisClubService,
    IUpdateTennisClubService updateTennisClubService,
    IDeleteTennisClubService deleteTennisClubService,
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    IAdminReadModelRepository adminReadModelRepository,
    IMemberReadModelRepository memberReadModelRepository
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<TennisClubReadModel>>> GetAllTennisClubs(
        [FromQuery] int pageSize = 0,
        int pageNumber = 1)
    {
        var tennisClubs = await tennisClubReadModelRepository.GetAllTennisClubs(pageSize, pageNumber);
        
        var metadata = new
        {
            pageSize,
            pageNumber
        };
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        
        return Ok(tennisClubs);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TennisClubReadModel>> GetTennisClubById(Guid id)
    {
        var tennisClub = await tennisClubReadModelRepository.GetTennisClubById(id);
        
        if (tennisClub == null)
        {
            return NotFound($"Tennis Club with id {id} not found!");
        }
        
        return Ok(tennisClub);
    }
    
    [HttpGet("{id:guid}/admins")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AdminReadModel>>> GetAdminsByTennisClubId(Guid id)
    {
        var admins = await adminReadModelRepository.GetAdminsByTennisClubId(id);
        return Ok(admins);
    }
    
    [HttpGet("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<MemberReadModel>>> GetMembersByTennisClubId(Guid id)
    {
        var members = await memberReadModelRepository.GetMembersByTennisClubId(id);
        return Ok(members);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> RegisterTennisClub(
        [FromBody] TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        var registeredTennisClubId = await registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);
        return CreatedAtAction(nameof(RegisterTennisClub), new { id = registeredTennisClubId }, registeredTennisClubId);
    }
    
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> UpdateTennisClub(
        Guid id,
        [FromBody] TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        if (tennisClubUpdateCommand.SubscriptionTierId != null)
        {
            var updatedTennisClubId =
                await updateTennisClubService.ChangeSubscriptionTier(id,
                    tennisClubUpdateCommand.SubscriptionTierId);
            return Ok(updatedTennisClubId);
        }
        
        if (tennisClubUpdateCommand.Name != null)
        {
            var updatedTennisClubId =
                await updateTennisClubService.ChangeName(id, tennisClubUpdateCommand.Name);
            return Ok(updatedTennisClubId);
        }
        
        return BadRequest("You have to provide either a name or a subscription tier!");
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> DeleteTennisClub(Guid id)
    {
        var deletedTennisClubId = await deleteTennisClubService.DeleteTennisClub(id);
        return Ok(deletedTennisClubId);
    }
    
    [HttpPost("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> LockTennisClub(Guid id)
    {
        var lockedTennisClubId = await updateTennisClubService.LockTennisClub(id);
        return Ok(lockedTennisClubId);
    }
    
    [HttpDelete("{id:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> UnlockTennisClub(Guid id)
    {
        var unlockedTennisClubId = await updateTennisClubService.UnlockTennisClub(id);
        return Ok(unlockedTennisClubId);
    }
}