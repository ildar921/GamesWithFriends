namespace GamesWithFriends.Core.Options;

public sealed class TokensLifetimes(short accessToken, short refreshToken)
{
    public short AccessToken => accessToken;
    public short RefreshToken => refreshToken;
}