namespace GamesWithFriends.Core.Options;

public sealed class TokensLifetimes(short accessToken, short refreshToken)
{
    public short AccessToken { get; init; } = accessToken;
    public short RefreshToken { get; init; } = refreshToken;
}