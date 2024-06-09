using ClubService.Domain.Api;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;
public class UserPassword
{
    public UserId UserId;
    public string HashedPassword;
    // needed by efcore
    private UserPassword()
    {
        
    }

    private UserPassword(Guid userId, string hashedPassword)
    {
 
        UserId = new UserId(userId);
        HashedPassword = hashedPassword;
    }

    public static UserPassword Create(Guid userId, string password, IPasswordHasherService passwordHasherService)
    {
        string hashedPassword = passwordHasherService.HashPassword(password);
        return new UserPassword(userId, hashedPassword);
    }
}