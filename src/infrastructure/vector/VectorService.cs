using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Data;
using model;
using System.Diagnostics.CodeAnalysis;

namespace infrastructure.vector
{
    [Experimental("Ahmar")]
    internal class VectorService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, VectorStore vectorStore) : IVectorService
    {
        public async Task SaveAsync(ProductModel productModel)
        {
            var vector = await embeddingGenerator.GenerateVectorAsync(productModel.Description, new EmbeddingGenerationOptions { Dimensions = 3072 });
            var collection = vectorStore.GetCollection<string, ProductVectorModel>("productsv");
            await collection.EnsureCollectionExistsAsync();
            var vectorModel = ToVectorModel(productModel, vector);          
            await collection.UpsertAsync(vectorModel); 
        }
        private ProductVectorModel ToVectorModel(ProductModel productModel, ReadOnlyMemory<float> vector)
        {
            ProductVectorModel vectorModel = new()
            {
                productId = productModel.Id.ToString(),
                productNameName = productModel.Name,
                Description = productModel.Description,
                DescriptionEmbedding = vector
            };
            return vectorModel;
        }

        private ProductModel ToModel(ProductVectorModel productModel)
        {
            ProductModel vectorModel = new()
            {
                Name = productModel.productNameName,
                Description = productModel.Description,
            };
            return vectorModel;
        }
        public async Task<IEnumerable<ProductModel>> Search(string description)
        {
            List<ProductModel> products = new ();
            var vector = await embeddingGenerator.GenerateVectorAsync(description, new EmbeddingGenerationOptions { Dimensions = 3072 });
            var collection = vectorStore.GetCollection<string, ProductVectorModel>("productsv");
            await collection.EnsureCollectionExistsAsync();
            var vectorSearchOptions = new VectorSearchOptions<ProductVectorModel>
            {
                VectorProperty = r => r.DescriptionEmbedding,
            };
            var searchResult =  collection.SearchAsync(vector, top: 2, vectorSearchOptions);
            await foreach (var result in searchResult)
            {
                if(result.Score < 0.34f) continue; 
                products.Add(ToModel(result.Record));
                Console.WriteLine(result.Record.productNameName);
            }
            return products;
        }
    }
}
