using System.Runtime.InteropServices.JavaScript;
using ClubService.Domain.Api;
using Microsoft.AspNetCore.Identity;

namespace ClubService.Infrastructure.Services;

public class PasswordHasherService : IPasswordHasherService
{

    private readonly IPasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new object() ,password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(new object(), hashedPassword, providedPassword);
        // TODO: handle rehashing
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}