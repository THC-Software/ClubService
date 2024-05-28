namespace ClubService.Application.Api.Exceptions;

public class TennisClubNotFoundException(Guid id) : Exception(string.Format(DefaultMessage, id))
{
    private const string DefaultMessage = "Tennis club with id {0} was not found.";
}