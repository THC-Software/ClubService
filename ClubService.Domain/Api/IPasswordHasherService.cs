namespace ClubService.Domain.Api;

public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}