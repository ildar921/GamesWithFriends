using System.IdentityModel.Tokens.Jwt;
using GamesWithFriends.Core.Enums;
using GamesWithFriends.Core.Options;
using GamesWithFriends.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using ClaimTypes = GamesWithFriends.Core.Enums.ClaimTypes;

namespace GamesWithFriends.Application.Services;

public sealed class AuthService(
    ICustomersRepository repo,
    IOptions<AuthOptions> authOptions,
    ITokenGeneratorService tokenGenerator) : IAuthService
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    public async Task<string?> GenerateTokenAsync(TokenType type, string username, string password)
    {
        if (!await repo.VerifyPasswordAsync(username, password))
            return null;

        return await tokenGenerator.GenerateTokenAsync(type, username);
    }

    public async Task<(string?, string?)> RefreshAsync(string refreshToken)
    {
        var decodedToken = await new JwtSecurityTokenHandler()
            .ValidateTokenAsync(refreshToken, _authOptions.ValidationParameters);

        if (decodedToken is null || !decodedToken.IsValid)
            return (null, null);

        var usernameClaim = decodedToken.Claims
            .FirstOrDefault(claim => claim.Key == nameof(ClaimTypes.Username));

        if (usernameClaim.Value is not string username)
            return (null, null);

        var newAccessToken = await tokenGenerator.GenerateTokenAsync(TokenType.Access, username);
        var newRefreshToken = await tokenGenerator.GenerateTokenAsync(TokenType.Refresh, username);

        return (newAccessToken, newRefreshToken);
    }
}