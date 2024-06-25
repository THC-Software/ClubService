namespace ClubService.Infrastructure.Configurations;

public class SmtpConfiguration
{
    public required string Host { get; init; }
    public int Port { get; init; }
    public required int PollingInterval { get; init; } // In seconds
}