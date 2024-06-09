using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class UserPassword(Guid userId, string hashedPassword)
{
    public UserId UserId = new UserId(userId);
    public string HashedPassword = hashedPassword;
}