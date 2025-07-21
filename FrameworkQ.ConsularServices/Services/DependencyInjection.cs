using FrameworkQ.ConsularServices.Users;
using Microsoft.Extensions.DependencyInjection;

namespace FrameworkQ.ConsularServices.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConsularServices(this IServiceCollection services)
        {
           
            
            services.AddTransient<Queue>();
            services.AddTransient<ServiceInfo>();
            services.AddTransient<Token>();
            services.AddTransient<User>();
            services.AddTransient<Role>();
            services.AddTransient<Permission>();
                
            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
                

            // Register the service manager
            services.AddScoped<ServiceManager>();

            return services;
        }
    }
}
