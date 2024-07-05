using ClubService.Application.Api;
using ClubService.Application.EventHandlers;
using ClubService.Application.EventHandlers.AdminEventHandlers;
using ClubService.Application.EventHandlers.MemberEventHandlers;
using ClubService.Application.EventHandlers.SubscriptionTierEventHandlers;
using ClubService.Application.EventHandlers.SystemOperatorEventHandlers;
using ClubService.Application.EventHandlers.TennisClubEventHandlers;
using ClubService.Application.EventHandlers.TournamentEventHandlers;

namespace ClubService.API.ApplicationConfigurations;

public static class EventHandlerExtensions
{
    public static void AddEventHandlerConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IEventHandler, SubscriptionTierCreatedEventHandler>();
        services.AddScoped<IEventHandler, TennisClubRegisteredEventHandler>();
        services.AddScoped<IEventHandler, TennisClubLockedEventHandler>();
        services.AddScoped<IEventHandler, TennisClubNameChangedEventHandler>();
        services.AddScoped<IEventHandler, TennisClubSubscriptionTierChangedEventHandler>();
        services.AddScoped<IEventHandler, TennisClubDeletedEventHandler>();
        services.AddScoped<IEventHandler, TennisClubUnlockedEventHandler>();
        services.AddScoped<IEventHandler, AdminRegisteredEventHandler>();
        services.AddScoped<IEventHandler, AdminDeletedEventHandler>();
        services.AddScoped<IEventHandler, AdminFullNameChangedEventHandler>();
        services.AddScoped<IEventHandler, MemberRegisteredEventHandler>();
        services.AddScoped<IEventHandler, MemberLockedEventHandler>();
        services.AddScoped<IEventHandler, MemberUnlockedEventHandler>();
        services.AddScoped<IEventHandler, MemberDeletedEventHandler>();
        services.AddScoped<IEventHandler, MemberFullNameChangedEventHandler>();
        services.AddScoped<IEventHandler, MemberEmailChangedEventHandler>();
        services.AddScoped<IEventHandler, TournamentConfirmedEventHandler>();
        services.AddScoped<IEventHandler, TournamentCanceledEventHandler>();
        services.AddScoped<IEventHandler, SystemOperatorRegisteredEventHandler>();
        services.AddScoped<ChainEventHandler>();
    }
}