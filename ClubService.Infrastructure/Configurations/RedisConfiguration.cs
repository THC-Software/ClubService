namespace ClubService.Infrastructure.Configurations;

public class RedisConfiguration
{
    public required string Host { get; init; }
    public required string StreamName { get; init; }
    public required string ConsumerGroup { get; init; }
}