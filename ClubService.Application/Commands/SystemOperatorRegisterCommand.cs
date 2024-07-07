using System.ComponentModel.DataAnnotations;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Commands;

public class SystemOperatorRegisterCommand(string username, string password)
{
    [Required]
    public string Username { get; } = username;

    [Required]
    public string Password { get; } = password;
}