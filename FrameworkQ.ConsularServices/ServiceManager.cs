using FrameworkQ.ConsularServices.Services;
using FrameworkQ.ConsularServices.Services;

namespace FrameworkQ.ConsularServices;

public class ServiceManager :  IManager<Service>
{
   
    private IServiceRepository _serviceRepository;

    public ServiceManager( IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public Task<Service> CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : Service
    {
        if (entity is not Service Service)
            throw new ArgumentException("Entity must be of type Service", nameof(entity));

        var createdService = _serviceRepository.CreateServiceInfo(Service);
        return Task.FromResult(createdService);
    }

    public Task<Service?> GetAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : Service
    {
        if (id is not long ServiceId)
            throw new ArgumentException("ID must be of type long", nameof(id));

        var Service = _serviceRepository.GetServiceInfo(ServiceId);
        return Task.FromResult<Service?>(Service);
    }

    public Task<bool> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : Service
    {
        if (entity is not Service Service)
            throw new ArgumentException("Entity must be of type Service", nameof(entity));

        _serviceRepository.UpdateServiceInfo(Service);

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : Service
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Service>> ListAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : Service
    {
        var Services = _serviceRepository.ListServiceInfo();
        return Task.FromResult<IReadOnlyList<Service>>(Services);
    }
    
}