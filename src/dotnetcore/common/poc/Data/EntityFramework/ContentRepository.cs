using System.Collections.Generic;
using Core.Data;
using Core.Entity;
using Entity;
using System.Threading.Tasks;
using System.Linq;
using Data.Context;

namespace Data.EntityFramework
{
    public class ContentRepository : IRepository<Content, int>
    {
        private readonly ContentContext _contentContext;

        public ContentRepository(ContentContext contentContext)
        {
            _contentContext = contentContext;
        }

        public async Task CreateAsync(IEntity<int> item)
        {
            await _contentContext.Content.AddAsync((Content)item).ConfigureAwait(false);
            await _contentContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public void Delete(int id)
        {
            var content = Get(id);
            _contentContext.Remove(content);
            _contentContext.SaveChanges();
        }

        public Content Get(int id)
        {
            var content = _contentContext.Content.First(q => q.Id == id);
            return content;
        }

        public IList<Content> GetAll()
        {
            var contents = _contentContext.Content.ToList();
            return contents;
        }

        public void Update(IEntity<int> item)
        {
            _contentContext.Content.Update((Content)item);
            _contentContext.SaveChanges();
        }
    }
}
