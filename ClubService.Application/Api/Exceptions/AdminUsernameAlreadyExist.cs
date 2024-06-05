namespace ClubService.Application.Api.Exceptions;

public class AdminUsernameAlreadyExist(string username, string tennisClubName, Guid tennisClubId) : Exception
{
    public override string Message { get; } =
        $"Admin username '{username}' already exist in tennis club '{tennisClubName}' ({tennisClubId}).";
}