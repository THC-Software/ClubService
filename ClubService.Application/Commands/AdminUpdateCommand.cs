using System.ComponentModel.DataAnnotations;

namespace ClubService.Application.Commands;

public class AdminUpdateCommand(string firstName, string lastName)
{
    [Required]
    public string FirstName { get; } = firstName;
    
    [Required]
    public string LastName { get; } = lastName;
}