namespace ClubService.Application.Api.Exceptions;

public class MemberNotFoundException(Guid id) : Exception
{
    public override string Message { get; } = $"Member with id {id} not found.";
}