using System.ComponentModel.DataAnnotations;

namespace ClubService.Application.Commands;

public class TennisClubRegisterCommand(
    string name,
    Guid subscriptionTierId,
    string username,
    string password,
    string firstName,
    string lastName)
{
    [Required]
    public string Name { get; } = name;

    [Required]
    public Guid SubscriptionTierId { get; } = subscriptionTierId;

    [Required]
    public string Username { get; } = username;

    [Required]
    public string Password { get; } = password;

    [Required]
    public string FirstName { get; } = firstName;

    [Required]
    public string LastName { get; } = lastName;
}