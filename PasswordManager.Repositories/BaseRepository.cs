using Microsoft.EntityFrameworkCore;
using PasswordManager.Database;
using PasswordManager.Database.Entities;
using PasswordManager.Repositories.Contracts;
using System.Linq.Expressions;

namespace PasswordManager.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : IdentityEntity
    {
        protected readonly PasswordManagerContext _context;

        public BaseRepository(PasswordManagerContext context)
        {
            _context = context;
        }

        public Task<T> GetByIdAsync(int id) => _context.Set<T>().FindAsync(id).AsTask();

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => _context.Set<T>().FirstOrDefaultAsync(predicate);

        public async Task<int> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task AddAsync(IEnumerable<T> entity)
        {
            await _context.Set<T>().AddRangeAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(T entity)
        {
            // In case AsNoTracking is used
            _context.Entry(entity).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }

        public Task UpdateAsync(T t, int entryId)
        {
            var local = _context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(t).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }

        public Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _context.UpdateRange(entities);
            return _context.SaveChangesAsync();
        }

        public Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
            return _context.SaveChangesAsync();
        }

        public Task RemoveAsync(T entity, int entryId)
        {
            var local = _context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(entity).State = EntityState.Deleted;
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public Task<int> CountAllAsync() => _context.Set<T>().CountAsync();

        public Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate)
            => _context.Set<T>().CountAsync(predicate);
    }
}