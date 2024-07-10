namespace ClubService.Infrastructure.Configurations;

public class SmtpConfiguration
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string SenderEmailAddress { get; init; }
    public required int PollingIntervalMilliSeconds { get; init; }
}