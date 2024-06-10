using System.ComponentModel.DataAnnotations;

namespace ClubService.Application.Commands;

public class MemberRegisterCommand(string firstName, string lastName, string email, string password, Guid tennisClubId)
{
    [Required]
    public string FirstName { get; } = firstName;
    
    [Required]
    public string LastName { get; } = lastName;
    
    [Required]
    public string Email { get; } = email;

    [Required]
    public string Password { get; } = password;

    [Required]
    public Guid TennisClubId { get; } = tennisClubId;
}