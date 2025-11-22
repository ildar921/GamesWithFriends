namespace GamesWithFriends.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void Configure()
        {
            builder.Configuration.Configure();
            builder.Services.Configure(builder.Configuration);
        }
    }
}