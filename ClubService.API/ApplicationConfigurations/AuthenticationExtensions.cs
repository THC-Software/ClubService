using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ClubService.API.ApplicationConfigurations;

public static class AuthenticationExtensions
{
    public static void AddAuthenticationConfigurations(this IServiceCollection service, IConfiguration configuration)
    {
        service
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    RequireSignedTokens = false,
                    SaveSigninToken = true,
                    RoleClaimType = "groups"
                };
            });

        service.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", policy => policy.RequireRole("ADMIN"))
            .AddPolicy("MemberPolicy", policy => policy.RequireRole("MEMBER"));
    }
}