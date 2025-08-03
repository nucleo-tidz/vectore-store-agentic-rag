using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Text;

namespace infrastructure.vector
{
    [Experimental("Ahmar")]
    public class DocumentService(VectorStore vectorStore) : IDocumentService
    {
        public async Task SaveAsync(string content)
        {
            using var textSearchStore = new TextSearchStore<string>(vectorStore, collectionName: "calculation-formulas", vectorDimensions: 3072, new()
            { SearchNamespace = "calculation/rules" });
            
            await textSearchStore.UpsertDocumentsAsync(Chunk(content));
        }
        private IEnumerable<TextSearchDocument> Chunk(string content)
        {
            var result = new List<TextSearchDocument>();            
            IEnumerable<string> chunks = TextChunker.SplitPlainTextLines(content, 50);
           
            foreach (string chunk in chunks)
            {

                result.Add(new TextSearchDocument
                {
                    Text = chunk,
                    Namespaces = ["calculation/rules"],
                    SourceLink= "Load Calculation.txt",
                    SourceName = "Load Calculation.txt"
                });
            }
            return result;
        }
    }
}
