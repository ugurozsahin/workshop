using Data.Context;
using Entity;

namespace Data.EntityFramework
{
    public class ProductModelRepository : BaseRepository<ProductModel, int>
    {
        public ProductModelRepository(ProductModelContext context)
            :base(context)
        {
        }
    }
}
