using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GamesWithFriends.Core.Enums;
using GamesWithFriends.Core.Options;
using GamesWithFriends.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

namespace GamesWithFriends.Application.Services;

public class TokenGeneratorService(ICustomersRepository repo, IOptions<AuthOptions> authOptions)
    : ITokenGeneratorService
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    public async Task<string?> GenerateTokenAsync(TokenType type, string username)
    {
        var role = await repo.GetRoleAsync(username);
        var expiresHours = type == TokenType.Access
            ? _authOptions.Lifetimes.AccessToken
            : _authOptions.Lifetimes.RefreshToken;

        if (role == null)
            return null;

        return GenerateToken(
            username,
            role.Value.ToString().ToLower(),
            expiresHours);
    }

    private string GenerateToken(string username, string role, short expiresHours)
    {
        var claims = new Claim[]
        {
            new(nameof(ClaimType.Username).ToLower(), username),
            new(nameof(ClaimType.Role).ToLower(), role)
        };

        var token = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims: claims,
            signingCredentials: _authOptions.SigningCredentials,
            expires: DateTime.UtcNow.AddHours(expiresHours)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}