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
    public async Task<ActionResult<List<SubscriptionTierReadModel>>> GetAllSubscriptionTiers()
    {
        var subscriptionTiers = await subscriptionTierReadModelRepository.GetAllSubscriptionTiers();
        return Ok(subscriptionTiers);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TennisClubReadModel>> GetSubscriptionTierById(string id)
    {
        var subscriptionTierGuid = new Guid(id);
        var subscriptionTier = await subscriptionTierReadModelRepository.GetSubscriptionTierById(subscriptionTierGuid);
        
        if (subscriptionTier == null)
        {
            return NotFound($"Subscription Tier with id {id} not found!");
        }
        
        return Ok(subscriptionTier);
    }
}