namespace ClubService.Application.Api.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid id, string username)
    {
        Message = $"User '{username}' not found in tennis club with id: {id}";
    }
    
    public UserNotFoundException(string message)
    {
        Message = message;
    }
    
    public override string Message { get; }
}