using GamesWithFriends.Core.Enums;

namespace GamesWithFriends.Application.Services;

public interface IAuthService
{
    public Task<string?> GenerateTokenAsync(TokenType type, string username, string password);
    public Task<(string?, string?)> RefreshAsync(string refreshToken);
}