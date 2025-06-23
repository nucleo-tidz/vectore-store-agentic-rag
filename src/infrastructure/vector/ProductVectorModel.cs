using Microsoft.Extensions.VectorData;

namespace infrastructure.vector
{
    public class ProductVectorModel
    {
        [VectorStoreKey]
        public required string productId { get; set; }
        [VectorStoreData]
        public required string productNameName { get; set; }
        [VectorStoreData(IsIndexed =true)]
        public required string Description { get; set; }
        [VectorStoreVector(3072)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
    }
}
