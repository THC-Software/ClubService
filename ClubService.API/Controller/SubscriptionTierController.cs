using Asp.Versioning;
using ClubService.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v{version:apiVersion}/subscriptionTiers")]
[ApiController]
[ApiVersion("1.0")]
public class SubscriptionTierController
{
    [HttpGet]
    public async Task<ActionResult<List<SubscriptionTierDto>>> GetAllSubscriptionTiers()
    {
        var subscriptionTiers = new List<SubscriptionTierDto>();
        return await Task.FromResult(subscriptionTiers);
    }
}