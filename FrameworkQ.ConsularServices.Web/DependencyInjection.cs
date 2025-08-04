using FrameworkQ.ConsularServices.Users;
using Microsoft.Extensions.DependencyInjection;
using FrameworkQ.ConsularServices;
using FrameworkQ.ConsularServices.Services;

namespace FrameworkQ.ConsularServices.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConsularServices(this IServiceCollection services)
        {
           
            
            services.AddTransient<Station>();
            services.AddTransient<ServiceInfo>();
            services.AddTransient<Token>();
            services.AddTransient<User>();
            services.AddTransient<Role>();
            services.AddTransient<Permission>();
                
            // Register repositories
            
            services.AddSingleton<ConnectionProvider>();
            services.AddScoped<TokenGenerator>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            
            // Register the service manager
            services.AddScoped<IServiceManager, ServiceManager>();

            return services;
        }
    }
}
