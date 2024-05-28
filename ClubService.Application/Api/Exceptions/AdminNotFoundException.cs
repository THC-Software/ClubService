namespace ClubService.Application.Api.Exceptions;

public class AdminNotFoundException(Guid id) : Exception(string.Format(DefaultMessage, id))
{
    private const string DefaultMessage = "Admin with id {0} was not found.";
}