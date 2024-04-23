namespace ClubService.Application.Commands;

public class AdminRegisterCommand(string username, string firstName, string lastName)
{
    public string Username { get; } = username;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}