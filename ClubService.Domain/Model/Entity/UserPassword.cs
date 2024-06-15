using ClubService.Domain.Api;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class UserPassword
{
    // needed by efcore
    private UserPassword()
    {
    }

    private UserPassword(Guid userId, string hashedPassword)
    {
        UserId = new UserId(userId);
        HashedPassword = hashedPassword;
    }

    public UserId UserId { get; } = null!;
    public string HashedPassword { get; }

    public static UserPassword Create(Guid userId, string password, IPasswordHasherService passwordHasherService)
    {
        var hashedPassword = passwordHasherService.HashPassword(password);
        return new UserPassword(userId, hashedPassword);
    }
}