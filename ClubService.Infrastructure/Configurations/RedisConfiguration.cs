namespace ClubService.Infrastructure.Configurations;

public class RedisConfiguration
{
    public required string Host { get; init; }
    public required int PollingIntervalMilliSeconds { get; init; }
    public required List<RedisStream> Streams { get; init; }
}