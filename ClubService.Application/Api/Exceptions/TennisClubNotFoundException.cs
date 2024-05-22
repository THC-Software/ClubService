namespace ClubService.Application.Api.Exceptions;

public class TennisClubNotFoundException : Exception
{
    public TennisClubNotFoundException()
    {
    }
    
    public TennisClubNotFoundException(string message) : base(message)
    {
    }
    
    public TennisClubNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}