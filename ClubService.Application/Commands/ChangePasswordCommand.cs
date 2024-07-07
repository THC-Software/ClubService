using System.ComponentModel.DataAnnotations;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Commands;

public class ChangePasswordCommand(UserId userId, string password)
{
    [Required]
    public UserId UserId { get; } = userId;

    [Required]
    public string Password { get; } = password;
}