using Microsoft.Extensions.DependencyInjection;

namespace FrameworkQ.ConsularServices.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConsularServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Register the service manager
            services.AddScoped<ServiceManager>();

            return services;
        }
    }
}
