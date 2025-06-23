using model;

namespace infrastructure.vector
{
    public interface IVectorService
    {
        Task SaveAsync(ProductModel productModel);
        Task<IEnumerable<ProductModel>> Search(string description);
    }
}
