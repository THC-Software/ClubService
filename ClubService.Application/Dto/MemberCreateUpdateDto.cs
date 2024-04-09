using System.ComponentModel.DataAnnotations;

namespace ClubService.Application.Dto;

public class MemberCreateUpdateDto(string firstName, string lastName, string email)
{
    [Required] public string FirstName { get; } = firstName;

    [Required] public string LastName { get; } = lastName;

    [Required] public string Email { get; } = email;
}