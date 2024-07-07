using System.ComponentModel.DataAnnotations;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Commands;

public class LoginCommand(string username, string password, TennisClubId? tennisClubId)
{
    [Required]
    public string Username { get; } = username;

    [Required]
    public string Password { get; } = password;

    public TennisClubId? TennisClubId { get; } = tennisClubId;
}