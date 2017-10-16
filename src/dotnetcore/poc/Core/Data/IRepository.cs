using Core.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Data
{
    public interface IRepository<T, TId>
    {
        Task CreateAsync(IEntity<TId> item);
        void Update(IEntity<TId> item);
        void Delete(TId id);
        IList<T> GetAll();
        T Get(TId id);
    }
}
