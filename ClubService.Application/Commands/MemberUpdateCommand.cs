namespace ClubService.Application.Commands;

public class MemberUpdateCommand(string? firstName, string? lastName, string? email)
{
    public string? FirstName { get; } = firstName;
    
    public string? LastName { get; } = lastName;
    
    public string? Email { get; } = email;
}