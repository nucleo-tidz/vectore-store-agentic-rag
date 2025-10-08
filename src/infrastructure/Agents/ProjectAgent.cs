using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Azure.AI.Agents.Persistent;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Text;

using StackExchange.Redis;

namespace infrastructure.Agents
{
    [Experimental("SKEXP0110")]
    public class ProjectAgent(Kernel _kernel, IConfiguration configuration, PersistentAgentsClient agentsClient) : AgentBase(_kernel, agentsClient), IProjectAgent
    {
        //private IEnumerable<TextSearchDocument> Chunk(string content)
        //{
        //    var result = new List<TextSearchDocument>();
        //    IEnumerable<string> chunks = TextChunker.SplitPlainTextLines(content, 50);
        //    foreach (string chunk in chunks)
        //    {

        //        result.Add(new TextSearchDocument
        //        {
        //            Text = chunk,
        //            Namespaces = ["calculation/rules"],
        //            SourceLink = "Load Calculation.txt",
        //            SourceName = "Load Calculation.txt"
        //        });
        //    }
        //    return result;
        //}

        //public async Task<string> Execute(string input, string userName, string threadId)
        //{


        //    var vectorStore = new InMemoryVectorStore(new() { EmbeddingGenerator = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>() });
        //    using var textSearchStore = new TextSearchStore<string>(vectorStore, collectionName: "formuala", vectorDimensions: 3072, new() { SearchNamespace = "calculation/rules" });
        //    await textSearchStore.UpsertDocumentsAsync(Chunk(File.ReadAllText("C:\\Workspace\\nucleo-tidz\\agentic-rag\\src\\Files\\Container Load Cost.txt")));
        //    var agentKernel = _kernel.Clone();
        //    ChatCompletionAgent agent =
        //   new()
        //   {
        //       Name = "Agent",
        //       Instructions = "you are a friendly agent",
        //       Kernel = _kernel,
        //   };
        //    string agentReply = string.Empty;
        //    ChatHistoryAgentThread agentThread = new();
        //    var textSearchProvider = new TextSearchProvider(textSearchStore);
        //    agentThread.AIContextProviders.Add(textSearchProvider);

        //    // how much is the specialhandling fees
        //    ChatMessageContent chatMessageContent = new(AuthorRole.User, input);
        //    await foreach (ChatMessageContent response in agent.InvokeAsync(chatMessageContent, agentThread))
        //    {
        //        agentReply = agentReply + response.Content;
        //    }
        //    return agentReply;
        //}

        public async Task<string> Execute(string input, string userName, string threadId)
        {
            var embedder = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var vectorStore =
                new RedisVectorStore(
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("redis")).GetDatabase(), new()
                    {
                        EmbeddingGenerator = embedder                      
                    });
            using var textSearchStore = new TextSearchStore<string>(vectorStore, collectionName: "formuala", vectorDimensions: 3072, new() { SearchNamespace = "calculation/rules" });
            var textSearchProvider = new TextSearchProvider(textSearchStore);
            ChatCompletionAgent agent =
           new()
           {
               Name = "Agent",
               Instructions = "you are a friendly agent",
               Kernel = _kernel,
           };
            string agentReply = string.Empty;
            ChatHistoryAgentThread agentThread = new();
            agentThread.AIContextProviders.Add(textSearchProvider);
            // how much is the specialhandling fees
            ChatMessageContent chatMessageContent = new(AuthorRole.User, input);
            await foreach (ChatMessageContent response in agent.InvokeAsync(chatMessageContent, agentThread))
            {
                agentReply = agentReply + response.Content;
            }
            return agentReply;
        }
    }

}
