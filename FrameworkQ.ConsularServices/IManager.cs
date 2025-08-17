namespace FrameworkQ.ConsularServices;

public interface IManager<T>
{
    Task<T> CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : T;
    Task<T?> GetAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : T;
    Task<bool> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : T;
    Task<bool> DeleteAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : T;
    Task<IReadOnlyList<T>> ListAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : T;
}

