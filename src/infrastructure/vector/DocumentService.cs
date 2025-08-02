using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Text;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace infrastructure.vector
{
    [Experimental("Ahmar")]
    public class DocumentService(VectorStore vectorStore) : IDocumentService
    {
        public async Task SaveAsync()
        {
            using var textSearchStore = new TextSearchStore<string>(vectorStore, collectionName: "product-desc", vectorDimensions: 3072, new()
            { SearchNamespace = "group/desc" });
            
            await textSearchStore.UpsertDocumentsAsync(Chunk());
        }
        private IEnumerable<TextSearchDocument> Chunk()
        {
            var result = new List<TextSearchDocument>();
            string data = File.ReadAllText("C:\\Workspace\\nucleo-tidz\\agentic-rag\\src\\Files\\product.txt");
            IEnumerable<string> chunks = TextChunker.SplitPlainTextLines(data, 50);
            foreach (string chunk in chunks)
            {

                result.Add(new TextSearchDocument
                {
                    Text = chunk,
                    Namespaces = ["group/personal"],
                    SourceLink= "C:\\Workspace\\nucleo-tidz\\agentic-rag\\src\\Files\\product.txt",
                    SourceName = "Ahmar.txt"
                });
            }
            return result;
        }
    }
}
