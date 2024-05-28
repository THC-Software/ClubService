namespace ClubService.Application.Api.Exceptions;

public class MemberNotFoundException(Guid id) : Exception(string.Format(DefaultMessage, id))
{
    private const string DefaultMessage = "Member with id {0} was not found.";
}