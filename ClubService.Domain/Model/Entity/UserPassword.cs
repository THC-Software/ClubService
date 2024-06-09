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

    public UserPassword(Guid userId, string password)
    {
 
        UserId = new UserId(userId);
        // TODO: Hash password
        HashedPassword = password;
    }
}