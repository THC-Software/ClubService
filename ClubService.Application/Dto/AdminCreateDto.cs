namespace ClubService.Application.Dto;

public class AdminCreateDto(string username, string firstName, string lastName)
{
    public string Username { get; } = username;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}