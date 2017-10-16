using System.Collections.Generic;
using Core.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace Data.EntityFramework
{
    public abstract class BaseRepository<T, TId> : IRepository<T, TId> where T : class
    {
        private readonly DbContext _context;

        private DbSet<T> Table { get { return _context.Set<T>(); } }

        protected BaseRepository(DbContext contentContext)
        {
            _context = contentContext;
        }

        public async Task CreateAsync(T entity)
        {
            await Table.AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(TId id)
        {
            var entity = await GetAsync(id).ConfigureAwait(false);
            Table.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<T> GetAsync(TId id)
        {
            var entity = await Table.FindAsync(id).ConfigureAwait(false);
            return entity;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var entities = await Table.ToListAsync().ConfigureAwait(false);
            return entities;
        }

        public async Task UpdateAsync(T entity)
        {
            Table.Update(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await Table.Where(predicate).ToListAsync().ConfigureAwait(false);
            return entities;
        }
    }
}
