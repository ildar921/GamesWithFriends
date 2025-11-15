using GamesWithFriends.Core.Enums;

namespace GamesWithFriends.Application.Services;

public interface ITokenGeneratorService
{
    public Task<string?> GenerateTokenAsync(TokenType type, string username);
}