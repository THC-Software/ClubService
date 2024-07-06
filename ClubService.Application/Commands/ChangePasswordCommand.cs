using System.ComponentModel.DataAnnotations;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Commands;

public class ChangePasswordCommand(Guid userId, string password)
{
    [Required]
    public UserId UserId { get; } = new(userId);

    [Required]
    public string Password { get; } = password;
}