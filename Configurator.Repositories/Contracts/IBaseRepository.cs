using Configurator.Database.Entities;
using System.Linq.Expressions;

namespace Configurator.Repositories.Contracts
{
    public interface IBaseRepository<T> where T : IdentityEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<int> AddAsync(T entity);
        Task AddAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateAsync(T t, int entryId);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task RemoveAsync(T entity, int entryId);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAllAsync();
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);
        Task<int> SaveChangesAsync();
    }
}
