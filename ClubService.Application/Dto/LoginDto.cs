using System.ComponentModel.DataAnnotations;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Dto;

public class LoginDto(string username, string password, TennisClubId tennisClubId)
{
    [Required]
    public string Username { get; } = username;

    [Required]
    public string Password { get; } = password;

    [Required]
    public TennisClubId TennisClubId { get; } = tennisClubId;
    
}