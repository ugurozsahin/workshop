using Core.Cache;
using Core.Constants;
using Core.Data;
using Entity;
using System.Linq;
using System.Threading.Tasks;

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
            Task.Run(_repository.GetAllAsync).ContinueWith(task =>
            {
                if (task.Result?.Any() == true)
                {
                    var allProducts = task.Result.ToDictionary(item => item.Id);
                    Task.Run(() => _cacher.SetAsync(GlobalConstants.CACHE_KEY_PRODUCT_MODELS, () => allProducts)).Wait();
                }
            }).Wait();
        }
    }
}
