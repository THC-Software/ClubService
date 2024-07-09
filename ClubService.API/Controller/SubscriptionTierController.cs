using Asp.Versioning;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/subscriptionTiers")]
[ApiController]
[ApiVersion("1.0")]
public class SubscriptionTierController(ISubscriptionTierReadModelRepository subscriptionTierReadModelRepository)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SubscriptionTierReadModel>>> GetAllSubscriptionTiers()
    {
        var subscriptionTiers = await subscriptionTierReadModelRepository.GetAllSubscriptionTiers();
        return Ok(subscriptionTiers);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("text/plain")]
    public async Task<ActionResult<TennisClubReadModel>> GetSubscriptionTierById(Guid id)
    {
        var subscriptionTier = await subscriptionTierReadModelRepository.GetSubscriptionTierById(id);
        
        if (subscriptionTier == null)
        {
            return NotFound($"Subscription Tier with id {id} not found!");
        }
        
        return Ok(subscriptionTier);
    }
}