namespace ClubService.Application.Api.Exceptions;

public class SubscriptionTierNotFoundException : Exception
{
    public SubscriptionTierNotFoundException()
    {
    }
    
    public SubscriptionTierNotFoundException(string? message) : base(message)
    {
    }
    
    public SubscriptionTierNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}