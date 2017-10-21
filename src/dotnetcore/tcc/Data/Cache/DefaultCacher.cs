using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System;
using Core.Cache;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

namespace Data.Cache
{
    public class DefaultCacher : ICacher
    {
        private static readonly object _lockObject = new object();
        private readonly IDistributedCache _distributedCache;

        public DefaultCacher(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var bytes = await _distributedCache.GetAsync(key).ConfigureAwait(false);
            var item = ByteArrayToObject<T>(bytes);
            return item;
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key).ConfigureAwait(false);
        }

        public async Task SetAsync<T>(string key, Func<T> itemFactory, int intervalInMinutes = 20)
        {
            var item = itemFactory.Invoke();
            if (EqualityComparer<T>.Default.Equals(item, default(T)))
                return;
            var bytes = ObjectToByteArray(item);
            await _distributedCache.SetAsync(key, bytes, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(intervalInMinutes)
            }).ConfigureAwait(false);
        }

        public T TryGetOrSet<T>(string key, Func<T> itemFactory, int intervalInMinutes = 20)
        {
            lock (_lockObject)
            {
                var cacheItem = default(T);
                var result = GetAsync<T>(key).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!EqualityComparer<T>.Default.Equals(result, default(T)))
                {
                    SetAsync(key, itemFactory, intervalInMinutes).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                return cacheItem;
            }
        }

        private static T ByteArrayToObject<T>(byte[] bytes)
        {
            if (bytes == null)
                return default(T);

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        private static byte[] ObjectToByteArray<T>(T item)
        {
            if (EqualityComparer<T>.Default.Equals(item, default(T)))
                return null;

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, item);
                return memoryStream.ToArray();
            }
        }
    }
}
