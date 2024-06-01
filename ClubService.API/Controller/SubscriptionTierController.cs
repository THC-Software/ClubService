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
}