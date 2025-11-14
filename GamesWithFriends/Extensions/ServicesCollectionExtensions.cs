namespace GamesWithFriends.Extensions;

public static class ServicesCollectionExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        services.ConfigureCommonServices();
        services.ConfigureAdminServices();
        services.ConfigureGraphQl();
    }

    private static void ConfigureCommonServices(this IServiceCollection services)
    {
    }

    private static void ConfigureAdminServices(this IServiceCollection services)
    {
    }

    private static void ConfigureGraphQl(this IServiceCollection services)
    {
    }
}