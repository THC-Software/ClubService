using Asp.Versioning;
using ClubService.Domain.ReadModel;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/subscriptionTiers")]
[ApiController]
[ApiVersion("1.0")]
public class SubscriptionTierController
{
    [HttpGet]
    public async Task<ActionResult<List<SubscriptionTierReadModel>>> GetAllSubscriptionTiers()
    {
        throw new NotImplementedException();
    }
}