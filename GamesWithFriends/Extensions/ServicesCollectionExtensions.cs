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
    extension(IServiceCollection services)
    {
        public void Configure(IConfiguration configuration)
        {
            services.ConfigureOptions(configuration);
            services.ConfigureDatabaseServices(configuration);
            services.ConfigureCommonServices();
            services.ConfigureAdminServices();
            services.ConfigureAuth();
            services.ConfigureApi();
        }

        private void ConfigureOptions(IConfiguration configuration)
        {
            services.Configure<AuthOptions>(configuration.GetSection("AuthOptions"));
            services.Configure<CookiePresetsOptions>(configuration.GetSection("CookiePresets"));
        }

        private void ConfigureDatabaseServices(IConfiguration configuration)
        {
            services.AddDbContextPool<AppDbContext>(options => options
                .UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

            services.AddTransient<ICustomersRepository, CustomersRepository>();
            services.AddTransient<IHasherService, HasherService>();
        }

        private void ConfigureCommonServices()
        {
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITokenGeneratorService, TokenGeneratorService>();
        }

        private void ConfigureAdminServices()
        {
        }

        private void ConfigureAuth()
        {
            var authOptions = services.BuildServiceProvider()
                .GetRequiredService<IOptions<AuthOptions>>().Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => { options.TokenValidationParameters = authOptions.ValidationParameters; });

            services.AddAuthorization();
        }

        private void ConfigureApi()
        {
            services.AddControllers();
        }
    }
}