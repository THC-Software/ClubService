namespace ClubService.Infrastructure.Configurations;

public class RedisStream
{
    public required string StreamName { get; init; }
    public required string ConsumerGroup { get; init; }
}