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

    public UserPassword(UserId userId, string hashedPassword)
    {
        UserId = userId;
        HashedPassword = hashedPassword;
    }
}