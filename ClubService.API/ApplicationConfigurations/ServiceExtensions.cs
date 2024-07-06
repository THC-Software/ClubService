using ClubService.Application.Api;
using ClubService.Application.Impl;
using ClubService.Domain.Api;
using ClubService.Infrastructure.Services;

namespace ClubService.API.ApplicationConfigurations;

public static class ServiceExtensions
{
    public static void AddServiceConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IRegisterMemberService, RegisterMemberService>();
        services.AddScoped<IUpdateMemberService, UpdateMemberService>();
        services.AddScoped<IDeleteMemberService, DeleteMemberService>();
        services.AddScoped<IRegisterTennisClubService, RegisterTennisClubService>();
        services.AddScoped<IUpdateTennisClubService, UpdateTennisClubService>();
        services.AddScoped<IDeleteTennisClubService, DeleteTennisClubService>();
        services.AddScoped<IRegisterAdminService, RegisterAdminService>();
        services.AddScoped<IDeleteAdminService, DeleteAdminService>();
        services.AddScoped<IUpdateAdminService, UpdateAdminService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IUserService, UserService>();
    }
}