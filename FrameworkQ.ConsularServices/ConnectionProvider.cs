using Microsoft.Extensions.Configuration;

namespace FrameworkQ.ConsularServices;

public class ConnectionProvider
{
    private readonly string _connectionString;

    public ConnectionProvider()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json");
        var configuration = builder.Build();
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}