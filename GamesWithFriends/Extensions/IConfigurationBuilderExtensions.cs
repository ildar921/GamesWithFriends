namespace GamesWithFriends.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static void Configure(this IConfigurationBuilder builder)
    {
        builder.ConfigureJsonFiles();
    }

    private static void ConfigureJsonFiles(this IConfigurationBuilder builder)
    {
        builder.AddJsonFile("Properties/Options/auth.json");
        builder.AddJsonFile("Properties/Options/database.json");
    }
}