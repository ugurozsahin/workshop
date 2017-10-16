using System;
using System.Threading.Tasks;

namespace Core.Cache
{
    public interface ICacher
    {
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task SetAsync<T>(string key, Func<T> itemFactory, int intervalInMinutes = 20);
        T TryGetOrSet<T>(string key, Func<T> itemFactory, int intervalInMinutes = 20);
    }
}
