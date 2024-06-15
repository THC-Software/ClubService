namespace ClubService.API.ApplicationConfigurations;

public static class SwaggerExtensions
{
    public static void AddSwaggerConfigurations(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }
}