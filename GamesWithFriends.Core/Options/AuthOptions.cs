// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GamesWithFriends.Core.Options;

public sealed record AuthOptions
{
    public bool ValidateIssuer { get; init; } = true;
    public bool ValidateAudience { get; init; } = true;
    public bool ValidateIssuerSigningKey { get; init; } = true;
    public bool ValidateLifetime { get; init; } = true;

    public string Issuer { get; init; } = "www.skybank.com";
    public string Audience { get; init; } = "www.skybank.com";
    public string IssuerSigningKey { get; init; } = "invalid_key";
    public TokensLifetimes Lifetimes { get; init; } = new(24, 360);

    public string CookieKey { get; init; } = "AspNetCore.Localization.Id";

    public SymmetricSecurityKey SymmetricSecurityKey =>
        new(Encoding.UTF8.GetBytes(IssuerSigningKey));

    public SigningCredentials SigningCredentials =>
        new(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);

    public TokenValidationParameters ValidationParameters =>
        new()
        {
            ValidateIssuer = ValidateIssuer,
            ValidateAudience = ValidateAudience,
            ValidateIssuerSigningKey = ValidateIssuerSigningKey,
            ValidateLifetime = ValidateLifetime,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = SymmetricSecurityKey
        };
}