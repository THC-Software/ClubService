namespace ClubService.Application.Api.Exceptions;

public class SubscriptionTierNotFoundException(Guid id) : Exception(string.Format(DefaultMessage, id))
{
    private const string DefaultMessage = "Subscription tier with id {0} was not found.";
}