using System.ComponentModel.DataAnnotations;

namespace ClubService.Application.Commands;

public class AdminRegisterCommand(string username, string firstName, string lastName, Guid tennisClubId)
{
    [Required]
    public string Username { get; } = username;
    
    [Required]
    public string FirstName { get; } = firstName;
    
    [Required]
    public string LastName { get; } = lastName;
    
    [Required]
    public Guid TennisClubId { get; } = tennisClubId;
}