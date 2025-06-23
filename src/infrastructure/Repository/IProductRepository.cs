using model;

namespace infrastructure.Repository
{
    public interface IProductRepository
    {
        Task<int> Save(ProductModel product);
    }
}
