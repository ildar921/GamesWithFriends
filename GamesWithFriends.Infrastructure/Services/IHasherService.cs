namespace GamesWithFriends.Infrastructure.Services;

public interface IHasherService
{
    public string Hash(string text);
    public bool Verify(string text, string hash);
}