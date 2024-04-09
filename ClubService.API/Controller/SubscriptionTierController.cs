using ClubService.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ClubService.API.Controller;

[Route("api/v1/subscriptionTiers")]
[ApiController]
public class SubscriptionTierController
{
    [HttpGet]
    public async Task<ActionResult<List<SubscriptionTierDto>>> GetAllSubscriptionTiers()
    {
        var subscriptionTiers = new List<SubscriptionTierDto>();
        return await Task.FromResult(subscriptionTiers);
    }
}