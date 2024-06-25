namespace ClubService.Application.Api.Exceptions;

public class TournamentNotFoundException(Guid id) : Exception
{
    public override string Message { get; } = $"Tournament with id {id} not found.";
}