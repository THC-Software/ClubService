namespace ClubService.Application.Api.Exceptions;

public class SubscriptionTierNotFoundException(Guid id) : Exception
{
    public override string Message { get; } = $"Subscription tier with id {id} not found.";
}