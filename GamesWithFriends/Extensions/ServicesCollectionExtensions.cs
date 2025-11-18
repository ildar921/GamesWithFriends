using GamesWithFriends.Application.Services;
using GamesWithFriends.Core.Options;
using GamesWithFriends.Infrastructure.Contexts;
using GamesWithFriends.Infrastructure.Repositories;
using GamesWithFriends.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GamesWithFriends.Extensions;

public static class ServicesCollectionExtensions
{
    public static void Configure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions(configuration);
        services.ConfigureDatabaseServices(configuration);
        services.ConfigureCommonServices();
        services.ConfigureAdminServices();
        services.ConfigureAuth();
        services.ConfigureGraphQl();
    }

    private static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection("AuthOptions"));
    }

    private static void ConfigureDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<AppDbContext>(options => options
            .UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddTransient<ICustomersRepository, CustomersRepository>();
        services.AddTransient<IHasherService, HasherService>();
    }

    private static void ConfigureCommonServices(this IServiceCollection services)
    {
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ITokenGeneratorService, TokenGeneratorService>();
    }

    private static void ConfigureAdminServices(this IServiceCollection services)
    {
    }

    private static void ConfigureAuth(this IServiceCollection services)
    {
        var authOptions = services.BuildServiceProvider()
            .GetRequiredService<IOptions<AuthOptions>>().Value;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => { options.TokenValidationParameters = authOptions.ValidationParameters; });

        services.AddAuthorization();
    }

    private static void ConfigureGraphQl(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddGraphQLServer()
            .AddAuthorization()
            .AddPagingArguments()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddGamesWithFriendsTypes();
    }
}