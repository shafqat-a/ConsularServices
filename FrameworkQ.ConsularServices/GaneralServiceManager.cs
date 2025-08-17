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

public class GeneralServiceManager : IServiceManager
{
    private IUserRepository _userRepository;
    private IServiceRepository _serviceRepository;
    public GeneralServiceManager(IUserRepository userRepository,
                            IServiceRepository serviceRepository)
    {
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
    }



    // // Service
    // public Task<IReadOnlyList<Service>> ListAsync(CancellationToken cancellationToken = default)
    // {
    //     var services = _serviceRepository.ListServiceInfo();
    //     return Task.FromResult<IReadOnlyList<Service>>(services);
    // }

    // public Task<Service> CreateAsync(Service service, CancellationToken cancellationToken = default)
    // {
    //     var created = _serviceRepository.CreateServiceInfo(service);
    //     return Task.FromResult(created);
    // }

    // public Task<Service?> GetAsync(object id, CancellationToken cancellationToken = default)
    // {
    //     var service = _serviceRepository.GetServiceInfo(id as long? ?? 0);
    //     return Task.FromResult<Service?>(service);
    // }

    // public Task UpdateAsync(Service entity, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }

    // public Task DeleteAsync(object id, CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }


    public User[] GetUsers()
    {
        return _userRepository.GetUsers();
    }

    public User GetUserByEmail(string email)
    {
        return _userRepository.GetUserByEmail(email);
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

    public override string ToString()
    {
        return "Service Manager at Consular";
    }




    
    
}