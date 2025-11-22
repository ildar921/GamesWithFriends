namespace GamesWithFriends.Extensions;

public static class ConfigurationBuilderExtensions
{
    extension(IConfigurationBuilder builder)
    {
        public void Configure()
        {
            builder.ConfigureJsonFiles();
        }

        private void ConfigureJsonFiles()
        {
            builder.AddJsonFile("Properties/Options/auth.json");
            builder.AddJsonFile("Properties/Options/cookie.json");
            builder.AddJsonFile("Properties/Options/database.json");
        }
    }
}