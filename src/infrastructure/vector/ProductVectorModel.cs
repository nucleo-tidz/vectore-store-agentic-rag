using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

namespace infrastructure.vector
{
    public class ProductVectorModel
    {
        [VectorStoreKey]
        [TextSearchResultName]
        public required string productId { get; set; }
        [VectorStoreData]
        [TextSearchResultValue]
        public required string productNameName { get; set; }
        [VectorStoreData(IsIndexed =true)]
        [TextSearchResultValue]
        public required string Description { get; set; }
        [VectorStoreVector(3072)]
        public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
    }
    public sealed class ProductVectorModelStringMapper : ITextSearchStringMapper
    {
        public string MapFromResultToString(object result)
        {
            if (result is ProductVectorModel dataModel)
            {
                return dataModel.Description;
            }
            throw new ArgumentException("Invalid result type.");
        }
    }

    public sealed class ProductVectorModelTextSearchResultMapper : ITextSearchResultMapper
    {
        public TextSearchResult MapFromResultToTextSearchResult(object result)
        {
            if (result is ProductVectorModel dataModel)
            {
                return new TextSearchResult(value: dataModel.Description)
                {
                   // Name = dataModel.productId.ToString()
                };
            }
            throw new ArgumentException("Invalid result type.");
        }
    }
}
