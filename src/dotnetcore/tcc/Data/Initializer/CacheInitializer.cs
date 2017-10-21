using Core.Cache;
using Core.Constants;
using Core.Data;
using Entity;
using System.Linq;

namespace Data.Initializer
{
    public class CacheInitializer
    {
        private readonly IRepository<ProductModel, int> _repository;
        private readonly ICacher _cacher;
        public CacheInitializer(
            IRepository<ProductModel, int> repository,
            ICacher cacher)
        {
            _repository = repository;
            _cacher = cacher;
        }

        public void Initialize()
        {
            var allProducts = _repository.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            if (allProducts?.Any() == true)
            {
                var dictAllProducts = allProducts.ToDictionary(item => item.Id);
                _cacher.SetAsync(GlobalConstants.CACHE_KEY_PRODUCT_MODELS, () => dictAllProducts)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
