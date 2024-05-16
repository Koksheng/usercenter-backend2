using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using usercenter.Application.Common.Interfaces.Authentication;
using usercenter.Application.Common.Interfaces.Persistence;
using usercenter.Application.Common.Interfaces.Services;
using usercenter.Infrastructure.Authentication;
using usercenter.Infrastructure.Persistence;
using usercenter.Infrastructure.Services;

namespace usercenter.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
