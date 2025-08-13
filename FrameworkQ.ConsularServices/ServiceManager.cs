using FrameworkQ.ConsularServices.Services;
using FrameworkQ.ConsularServices.Users;

namespace FrameworkQ.ConsularServices;

public interface IServiceManager
{
    User GetUserByEmail(string email);
    User[] GetUsers();
    User GetUserById(long userId);

    Station[] GetStations();
    Station GetStation(long station_id);

    Service[] ListServiceInfo();
    Service GetService(int serviceId);
}
public class ServiceManager : IServiceManager
{
    private IUserRepository _userRepository;
    private IServiceRepository _serviceRepository;
    public ServiceManager(IUserRepository userRepository,
                            IServiceRepository serviceRepository)
    {
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
    }

    public User GetUserByEmail(string email)
    {
        return _userRepository.GetUserByEmail(email);
    }

    public User[] GetUsers()
    {
        return _userRepository.GetUsers();
    }
    
    public Station[] GetStations()
    {
        return _serviceRepository.GetStations();
    }

    public User GetUserById(long userId)
    {
        return _userRepository.GetUser(userId);
    }

    public Station GetStation(long station_id)
    {
        return _serviceRepository.GetStation(station_id);
    }

    public Service[] ListServiceInfo()
    {
        return _serviceRepository.ListServiceInfo();
    }

    public Service GetService(int serviceId)
    {
        return _serviceRepository.GetServiceInfo(serviceId);
    }
}