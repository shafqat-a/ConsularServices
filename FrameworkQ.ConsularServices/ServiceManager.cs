using FrameworkQ.ConsularServices.Services;
using FrameworkQ.ConsularServices.Users;

namespace FrameworkQ.ConsularServices;

public interface IServiceManager
{
    User GetUserByEmail(string email);
    User[] GetUsers();
}
public class ServiceManager : IServiceManager
{
    private IUserRepository _userRepository;
    private IServiceRepository _serviceRepository;
    public ServiceManager ( IUserRepository userRepository,
                            IServiceRepository serviceRepository )
    {
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
    }
    
    public User GetUserByEmail (string email)
    {
        return _userRepository.GetUserByEmail(email);
    }

    public User[] GetUsers()
    {
        return _userRepository.GetUsers();
    }
}