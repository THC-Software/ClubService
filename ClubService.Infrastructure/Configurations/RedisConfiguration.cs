namespace ClubService.Infrastructure.Configurations;

public class RedisConfiguration
{
    public required string Host { get; init; }
    public required int PollingInterval { get; init; } // In seconds
    public required List<RedisStream> Streams { get; init; }
}