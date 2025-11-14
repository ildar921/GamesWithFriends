namespace GamesWithFriends.Infrastructure.Services;

public class HasherService : IHasherService
{
    public string Hash(string text)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(text);
    }

    public bool Verify(string text, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(text, hash);
    }
}