using Core.Cache;
using Core.Constants;
using Core.Data;
using Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Cache
{
    public class ProductModelCacheManager : IProductModelCacheManager
    {
        private readonly ICacher _cacher;
        private readonly ILogger<ProductModelCacheManager> _logger;
        private readonly IRepository<ProductModel, int> _repository;
        public ProductModelCacheManager(
            IRepository<ProductModel, int> repository,
            ICacher cacher,
            ILogger<ProductModelCacheManager> logger)
        {
            _repository = repository;
            _cacher = cacher;
            _logger = logger;
        }

        public void UpdateCache()
        {
            UpdateCacheAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public async Task UpdateCacheAsync()
        {
            _logger.LogTrace("Update Cache Started!!!");
            var maxLastUpdatedTime = await _cacher.GetAsync<DateTime?>(GlobalConstants.CACHE_KEY_MAX_LAST_UPDATED_TIME).ConfigureAwait(false) ?? new DateTime?(DateTime.MinValue);
            var productModels = await _repository.GetFilteredAsync(q => (!q.LastUpdatedTime.HasValue || q.LastUpdatedTime > maxLastUpdatedTime)).ConfigureAwait(false);
            if (productModels?.Any() == true)
            {
                var cachedProductModels = await _cacher.GetAsync<Dictionary<int, ProductModel>>(GlobalConstants.CACHE_KEY_PRODUCT_MODELS).ConfigureAwait(false);

                UpdateCachedProductModels(cachedProductModels, productModels);

                await _cacher.SetAsync(GlobalConstants.CACHE_KEY_PRODUCT_MODELS, () => cachedProductModels).ConfigureAwait(false);

                _logger.LogTrace($"Changed Product Count : {productModels.Count}");
            }
            await _cacher.SetAsync(GlobalConstants.CACHE_KEY_MAX_LAST_UPDATED_TIME, () => new DateTime?(DateTime.Now)).ConfigureAwait(false);
            _logger.LogTrace("Update Cache Ended!!!");
        }

        public void UpdateCachedProductModels(Dictionary<int, ProductModel> cachedProductModels, List<ProductModel> diffProductModels)
        {
            if (cachedProductModels?.Count > 0)
            {
                foreach (var productModel in diffProductModels)
                {
                    if (cachedProductModels.ContainsKey(productModel.Id))
                    {
                        cachedProductModels[productModel.Id] = productModel;
                    }
                    else
                    {
                        cachedProductModels.Add(productModel.Id, productModel);
                    }
                }
            }
            else
            {
                cachedProductModels = diffProductModels.ToDictionary(productModel => productModel.Id);
            }
        }
    }
}
