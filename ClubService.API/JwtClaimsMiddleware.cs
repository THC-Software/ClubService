using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClubService.API;

public class JwtClaimsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    var jwtToken = new JwtSecurityToken(token);
                    var claims = jwtToken.Claims.ToList();

                    // Add the Role claims if groups are used as roles
                    var roleClaims = claims.Where(c => c.Type == "groups")
                        .Select(c => new Claim(ClaimTypes.Role, c.Value))
                        .ToList();

                    // Add role claims to the existing claims list
                    claims.AddRange(roleClaims);

                    var claimsIdentity = new ClaimsIdentity(claims, "jwt");
                    context.User = new ClaimsPrincipal(claimsIdentity);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
        }

        await next(context);
    }
}