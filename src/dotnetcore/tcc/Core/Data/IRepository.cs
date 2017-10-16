using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Data
{
    public interface IRepository<T, TId>
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(TId id);
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(TId id);
        Task<List<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate);
    }
}
