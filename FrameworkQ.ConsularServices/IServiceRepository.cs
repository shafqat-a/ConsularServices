using FrameworkQ.ConsularServices.Services;

namespace FrameworkQ.ConsularServices;

public interface IServiceRepository
{
    // ServiceInfo CRUD
    void CreateServiceInfo(ServiceInfo service);
    ServiceInfo GetServiceInfo(int id);
    void UpdateServiceInfo(ServiceInfo service);
    void DeleteServiceInfo(int id);

    // Token CRUD
    void CreateToken(Token token);
    Token GetToken(string tokenId);
    void UpdateToken(Token token);
    void DeleteToken(string tokenId);
}