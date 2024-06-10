namespace ClubService.Application.Api.Exceptions;

public class UserNotFoundException : Exception
{
    public override string Message { get; }

    public UserNotFoundException(Guid id, string username)
    {
        Message = $"{username} not found in tennis club with id: {id}";
    }
    public UserNotFoundException(string message) 
    {
        Message = message;
    }
}