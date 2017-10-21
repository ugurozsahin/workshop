using System.Collections.Generic;
using Core.Data;
using Core.Entity;
using Entity;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

namespace Data.Redis
{
    public class ContentRepository : IRepository<Content, int>
    {
        private readonly IDistributedCache _distributedCache;

        public ContentRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CreateAsync(IEntity<int> item)
        {
            var serializedObject = JsonConvert.SerializeObject((Content)item);
            await _distributedCache.SetStringAsync($"poc-{item.Id}", serializedObject, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(20)
            }).ConfigureAwait(false);
        }

        public void Delete(int id)
        {
            _distributedCache.Remove($"poc-{id}");
        }

        public Content Get(int id)
        {
            var serializedObject = _distributedCache.GetString($"poc-{id}");
            if (serializedObject != null)
            {
                var content = JsonConvert.DeserializeObject<Content>(serializedObject);
                return content;
            }
            return null;
        }

        public IList<Content> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(IEntity<int> item)
        {
            Task.Run(() => CreateAsync(item));
        }
    }
}
