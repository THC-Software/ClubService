namespace ClubService.Application.Api.Exceptions;

public class MemberEmailAlreadyExistsException(string email, string tennisClubName, Guid tennisClubId) : Exception
{
    public override string Message { get; } =
        $"Member e-mail address '{email}' already exists in tennis club '{tennisClubName}' ({tennisClubId}).";
}