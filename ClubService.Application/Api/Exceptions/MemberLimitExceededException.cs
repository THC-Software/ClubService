namespace ClubService.Application.Api.Exceptions;

public class MemberLimitExceededException(int maxMemberCount) : Exception
{
    public override string Message { get; } = $"Member limit of {maxMemberCount} exceeded.";
}