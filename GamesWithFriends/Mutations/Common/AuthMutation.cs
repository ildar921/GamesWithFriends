using GamesWithFriends.Application.Services;
using GamesWithFriends.Core.Enums;
using GamesWithFriends.Core.Options;
using GamesWithFriends.Mutations.Inputs;
using GamesWithFriends.Mutations.Payloads;
using HotChocolate.Resolvers;
using Microsoft.Extensions.Options;

namespace GamesWithFriends.Mutations.Common;

[MutationType]
public static class AuthMutation
{
    public static async Task<AuthenticatePayload?> AuthenticateAsync(AuthenticateInput input,
        IHttpContextAccessor accessor, IResolverContext context,
        [Service] IAuthService authService, [Service] IOptions<AuthOptions> options)
    {
        if (accessor.HttpContext is null)
        {
            context.ReportError(ErrorBuilder
                .New()
                .SetCode("CANNOT_ACCESS_HTTP_CONTEXT")
                .SetMessage("Cannot access HTTP Context")
                .Build());

            return null;
        }

        var authOptions = options.Value;

        var newAccessToken = await authService.GenerateTokenAsync(
            TokenType.Access,
            input.Username,
            input.Password);

        var newRefreshToken = await authService.GenerateTokenAsync(
            TokenType.Refresh,
            input.Username,
            input.Password);

        if (string.IsNullOrWhiteSpace(newAccessToken) ||
            string.IsNullOrWhiteSpace(newRefreshToken))
        {
            context.ReportError(ErrorBuilder
                .New()
                .SetCode("CANNOT_GENERATE_TOKENS")
                .SetMessage("Cannot generate tokens")
                .Build());

            return null;
        }

        AddSecureHttpOnlyCookie(accessor, authOptions.CookieKey, newRefreshToken);

        return new AuthenticatePayload(newAccessToken);
    }

    public static async Task<RefreshPayload?> RefreshAsync(IHttpContextAccessor accessor, IResolverContext context,
        [Service] IAuthService authService, [Service] IOptions<AuthOptions> options)
    {
        if (accessor.HttpContext is null)
        {
            context.ReportError(ErrorBuilder
                .New()
                .SetCode("CANNOT_ACCESS_HTTP_CONTEXT")
                .SetMessage("Cannot access HTTP Context")
                .Build());

            return null;
        }

        var authOptions = options.Value;

        var refreshToken = accessor.HttpContext.Request.Cookies[authOptions.CookieKey];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            context.ReportError(ErrorBuilder
                .New()
                .SetCode("CANNOT_ACCESS_REFRESH_TOKEN")
                .SetMessage("Cannot access refresh token")
                .Build());

            return null;
        }

        var (newAccessToken, newRefreshToken) = await authService.RefreshAsync(refreshToken);

        if (string.IsNullOrWhiteSpace(newAccessToken) ||
            string.IsNullOrWhiteSpace(newRefreshToken))
        {
            context.ReportError(ErrorBuilder
                .New()
                .SetCode("CANNOT_GENERATE_TOKENS")
                .SetMessage("Cannot generate tokens")
                .Build());

            return null;
        }

        AddSecureHttpOnlyCookie(accessor, authOptions.CookieKey, newRefreshToken);

        return new RefreshPayload(newAccessToken);
    }

    private static void AddSecureHttpOnlyCookie(IHttpContextAccessor accessor, string key, string value)
    {
        accessor.HttpContext?.Response.Cookies
            .Append(key, value, new CookieOptions
            {
                Secure = true,
                HttpOnly = true
            });
    }
}