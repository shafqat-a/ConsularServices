using FrameworkQ.ConsularServices.Services;
using FrameworkQ.ConsularServices.Users;

namespace FrameworkQ.ConsularServices;

public class UserManager :  IManager<User>
{
    private IUserRepository _userRepository;
      public UserManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;

    }

    public Task<User> CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : User
    {
        if (entity is not User user)
            throw new ArgumentException("Entity must be of type User", nameof(entity));

        var createdUser = _userRepository.CreateUser(user);
        return Task.FromResult(createdUser);
    }

    public Task<User?> GetAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : User
    {
        if (id is not long userId)
            throw new ArgumentException("ID must be of type long", nameof(id));

        var user = _userRepository.GetUser(userId);
        return Task.FromResult<User?>(user);
    }

    public Task<bool> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : User
    {
        if (entity is not User user)
            throw new ArgumentException("Entity must be of type User", nameof(entity));

        _userRepository.UpdateUser(user);

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : User
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<User>> ListAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : User
    {
        var users = _userRepository.GetUsers();
        return Task.FromResult<IReadOnlyList<User>>(users);
    }
    
}