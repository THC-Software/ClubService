using System.ComponentModel.DataAnnotations;

namespace ClubService.Application.Commands;

public class TennisClubRegisterCommand(string name, string subscriptionTierId)
{
    [Required]
    public string Name { get; } = name;
    
    [Required]
    public string SubscriptionTierId { get; } = subscriptionTierId;
}