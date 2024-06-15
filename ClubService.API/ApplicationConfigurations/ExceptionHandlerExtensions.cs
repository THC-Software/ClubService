namespace ClubService.API.ApplicationConfigurations;

public static class ExceptionHandlerExtensions
{
    public static void AddExceptionHandlerConfigurations(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }
}