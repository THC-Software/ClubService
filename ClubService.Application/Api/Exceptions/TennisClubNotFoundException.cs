namespace ClubService.Application.Api.Exceptions;

public class TennisClubNotFoundException(Guid id) : Exception
{
    public override string Message { get; } = $"Tennis club with id {id} not found.";
}