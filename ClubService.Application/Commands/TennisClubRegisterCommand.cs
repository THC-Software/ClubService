using System.ComponentModel.DataAnnotations;

namespace ClubService.Application.Commands;

public class TennisClubRegisterCommand(string name, Guid subscriptionTierId)
{
    [Required]
    public string Name { get; } = name;
    
    [Required]
    public Guid SubscriptionTierId { get; } = subscriptionTierId;
}