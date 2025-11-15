namespace GamesWithFriends.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void Configure(this WebApplicationBuilder builder)
    {
        builder.Configuration.Configure();
        builder.Services.Configure(builder.Configuration);
    }
}