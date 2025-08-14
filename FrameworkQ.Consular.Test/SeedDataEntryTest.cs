using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FrameworkQ.ConsularServices.Web;
using FrameworkQ.ConsularServices.Users;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using FrameworkQ.ConsularServices;
using FrameworkQ.ConsularServices.Services;
using Npgsql;

namespace FrameworkQ.Consular.Test;

public class SeedDataEntryTest : IDisposable
{
    private readonly IHost _host;
    private readonly IServiceScope _scope;
    private readonly IServiceProvider _serviceProvider;

    public SeedDataEntryTest()
    {
        _host = Setup();
        _scope = _host.Services.CreateScope();
        _serviceProvider = _scope.ServiceProvider;
    }

    private IHost Setup()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);
                
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "script.sql");
                if (File.Exists(scriptPath))
                {
                    string script = File.ReadAllText(scriptPath);
                    var conn = new NpgsqlConnection(connectionString);
                    conn.Execute(script);
                }
                    
                DependencyInjection.AddConsularServices(services, connectionString!);
            })
            .Build();
        ColumnMapper<User>.MapTypes();

        return host;
    }

    [Fact]
    public void Setup_Should_Configure_Services()
    {
        // Verify services are registered
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        Assert.NotNull(userRepository);
        
        var serviceRepository = _serviceProvider.GetRequiredService<IServiceRepository>();
        Assert.NotNull(serviceRepository);
    }

    private string Base64ToHex (string inputb64)
    {
        // Decode the Base64 string into a byte array.
        byte[] bytes = Convert.FromBase64String(inputb64);
        
        // Use a StringBuilder for efficient string construction.
        var hexBuilder = new StringBuilder(bytes.Length * 2);
        
        // Loop through each byte and convert it to its two-character hex representation.
        foreach (byte b in bytes)
        {
            // "X2" format specifier ensures uppercase hex and padding with a zero if necessary.
            hexBuilder.Append(b.ToString("X2"));
        }
        
        return hexBuilder.ToString();
    }
    
    [Fact]
    public void Should_Create_Admin_User_With_All_Permissions()
    {
        // Arrange
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var adminPassword = "Orion123@";
        var adminPasswordHash = Base64ToHex( CreateSha256Hash(adminPassword));

        // Step 1: Create admin user
        var adminUser = new User
        {
            Name = "admin",
            Email = "admin@example.com",
            PasswordHash = adminPasswordHash
        };
        var retUser = userRepository.CreateUser(adminUser);

        // Get the created user to get the UserId
        var createdUser = userRepository.GetUserByEmail("admin@example.com");
        Assert.NotNull(createdUser);

        // Step 2: Create admin role
        var adminRole = new Role
        {
            RoleName = "admin"
        };
        var retRole = userRepository.CreateRole(adminRole);

        // Get the created role using the returned ID
        var createdRole = userRepository.GetRole(retRole.RoleID);
        Assert.NotNull(createdRole);
        Assert.Equal("admin", createdRole.RoleName);

        // Step 3: Get all existing permissions from database
        var allPermissions = userRepository.GetAllPermissions();
        
        // If no permissions exist, create some sample permissions
        if (!allPermissions.Any())
        {
            var samplePermissions = new List<Permission>
            {
                new Permission { PermissionID = 1, PermissionName = "read" },
                new Permission { PermissionID = 2, PermissionName = "write" },
                new Permission { PermissionID = 3, PermissionName = "delete" },
                new Permission { PermissionID = 4, PermissionName = "admin" }
            };

            foreach (var permission in samplePermissions)
            {
                userRepository.CreatePermission(permission);
            }
            
            // Retrieve all permissions again after creating them
            allPermissions = userRepository.GetAllPermissions();
        }

        // Step 4: Assign all permissions to admin role
        var permissionIds = allPermissions.Select(p => (long)p.PermissionID).ToList();
        userRepository.AssignPermissionsToRole(createdRole.RoleID, permissionIds);

        // Step 5: Assign admin user to admin role
        userRepository.AssignRolesToUser(createdUser.UserId, new List<long> { createdRole.RoleID });

        // Assert
        var retrievedUser = userRepository.GetUser(createdUser.UserId);
        Assert.NotNull(retrievedUser);
        Assert.Equal("admin", retrievedUser.Name);
        Assert.Equal(adminPasswordHash, retrievedUser.PasswordHash);

        var retrievedRole = userRepository.GetRole(createdRole.RoleID);
        Assert.NotNull(retrievedRole);
        Assert.Equal("admin", retrievedRole.RoleName);
        
        // Verify that permissions were assigned
        Assert.True(allPermissions.Count > 0, "Should have at least one permission assigned to admin role");

        // Create a User role
        var roleUser = new Role() { RoleName = "User" };
        roleUser = userRepository.CreateRole(roleUser);
        
        userRepository.AddPermissionsToRole(roleUser.RoleID, new List<long>
        {
            Permission.UPDATE_SERVICE_INFO,
            Permission.UPDATE_TOKEN,
            Permission.UPDATE_SERVICE_INSTANCE
        });

        var userShafqat = new User()
            { Name = "Shafqat Ahmed", Email = "shafqatahmed@gmail.com", PasswordHash = Base64ToHex(CreateSha256Hash("Orion123@")) };
        userShafqat = userRepository.CreateUser(userShafqat);

        userRepository.AssignRolesToUser(userShafqat.UserId, new List<long> { roleUser.RoleID });
        
        var serviceRepository = _serviceProvider.GetRequiredService<IServiceRepository>();
        var sinfo = serviceRepository.CreateServiceInfo(new Service()
        {
            ServiceName = "New E-Passport - 120 pages",
            ServiceDescription = "Get a new e-passport of 120 pages",
            UsualServiceDays = 7,
            ServiceFee = 60
        });

        var sinfo2 = serviceRepository.CreateServiceInfo(new Service()
        {
            ServiceName = "New E-Passport - 68 pages",
            ServiceDescription = "Get a new e-passport of 68 pages",
            UsualServiceDays = 3,
            ServiceFee = 30
        });

        var token = serviceRepository.CreateToken(new Token()
        {
            TokenId = "temp", // This will be overridden by the repository
            Description = "Some Token",
            MobileNo = "000000000000", 
            Email = "shafqat@nuarca.com",
            GeneratedAt = DateTime.UtcNow,
            AppointmentAt = DateTime.UtcNow.AddDays(1),
            CompletedAt = DateTime.UtcNow.AddDays(2),
            ServiceType = new []{sinfo.ServiceId}
        });

        var station = serviceRepository.CreateStation(new Station()
        {
            StationName = "Station 1",
            Status = 0
        });


    }

    private static string CreateSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    public void Dispose()
    {
        _scope.Dispose();
        _host.Dispose();
    }
}