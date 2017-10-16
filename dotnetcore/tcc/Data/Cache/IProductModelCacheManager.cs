using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;

namespace Data.Cache
{
    public interface IProductModelCacheManager
    {
        void UpdateCache();
        Task UpdateCacheAsync();
        void UpdateCachedProductModels(Dictionary<int, ProductModel> cachedProductModels, List<ProductModel> diffProductModels);
    }
}