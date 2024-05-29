namespace ClubService.Application.Api.Exceptions;

public class AdminNotFoundException(Guid id) : Exception
{
    public override string Message { get; } = $"Admin with id {id} not found.";
}