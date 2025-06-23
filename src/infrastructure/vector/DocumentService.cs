using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Text;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.vector
{
    [Experimental("Ahmar")]
    public class DocumentService(Kernel kernel, IConfiguration configuration) : IDocumentService
    {
        public async Task SaveAsync()
        {
           var embeddingGenerator= kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var vectorStore =
                new RedisVectorStore(
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("redis")).GetDatabase(), new()
                    {
                        EmbeddingGenerator = embeddingGenerator,
                    });

            using var textSearchStore = new TextSearchStore<string>(vectorStore, collectionName: "personal-data", vectorDimensions: 3072, new()
            { SearchNamespace = "group/personal" });
            
            await textSearchStore.UpsertDocumentsAsync(Chunk());
        }
        private IEnumerable<TextSearchDocument> Chunk()
        {
            var result = new List<TextSearchDocument>();
            string data = File.ReadAllText("C:\\Personal-Workspace\\agentic-rag\\src\\Files\\Ahmar.txt");
            IEnumerable<string> chunks = TextChunker.SplitPlainTextLines(data, 50);
            foreach (string chunk in chunks)
            {

                result.Add(new TextSearchDocument
                {
                    Text = chunk,
                    
                    Namespaces = ["group/personal"],
                    SourceLink= "C:\\Personal-Workspace\\agentic-rag\\src\\Files\\Ahmar.txt",
                    SourceName = "Ahmar.txt"
                });
            }
            return result;
        }
    }
}
