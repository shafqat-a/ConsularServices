using FrameworkQ.ConsularServices.Users;
using Microsoft.Extensions.DependencyInjection;
using FrameworkQ.ConsularServices;
using FrameworkQ.ConsularServices.Services;
using Microsoft.EntityFrameworkCore;
using FrameworkQ.ConsularServices.Data;

namespace FrameworkQ.ConsularServices.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConsularServices(this IServiceCollection services, string connectionString)
        {
            // Add Entity Framework
            services.AddDbContext<ConsularDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            services.AddTransient<Station>();
            services.AddTransient<Service>();
            services.AddTransient<Token>();
            services.AddTransient<User>();
            services.AddTransient<Role>();
            services.AddTransient<Permission>();
                
            // Register repositories with EF Core versions
            services.AddSingleton<ConnectionProvider>(); // Keep for backward compatibility if needed
            services.AddScoped<TokenGenerator>();
            services.AddScoped<IUserRepository, UserRepositoryEF>();
            services.AddScoped<IServiceRepository, ServiceRepositoryEF>();
            
            // Register the service manager
            services.AddScoped<IServiceManager, GeneralServiceManager>();
            services.AddScoped<IManager<User>, UserManager>();
            services.AddScoped<IManager<Service>, ServiceManager>();

            return services;
        }
    }
}
