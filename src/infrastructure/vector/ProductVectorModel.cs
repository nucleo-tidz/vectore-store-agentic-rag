using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

namespace infrastructure.vector
{
    public class ProductVectorModel
    {
        [VectorStoreKey]
        public required string productId { get; set; }

        //[TextSearchResultName]
        [VectorStoreData]
        public required string productNameName { get; set; }

        //[TextSearchResultValue]
        [VectorStoreData(IsIndexed =true)]
    
        public required string Description { get; set; }

        [VectorStoreVector(3072)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
    }
}
