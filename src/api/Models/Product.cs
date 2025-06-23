using model;

namespace api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public static class ProductConvertor
    {
        public static ProductModel ToModel(this Product product)
        {
           return new ProductModel
           {
               Id = product.Id,
               Name = product.Name,
               Description = product.Description
           };
        }
    }
}
