﻿using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Data;

using StackExchange.Redis;

namespace infrastructure.Agents
{
    [Experimental("SKEXP0110")]
    public class ProjectAgent(Kernel _kernel, IConfiguration configuration) : AgentBase(_kernel, configuration), IProjectAgent
    {
        public async Task<string> Execute(string message)
        {
            var embedder = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
            var vectorStore =
                new RedisVectorStore(
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("redis")).GetDatabase(), new()
                    {
                        EmbeddingGenerator = embedder,
                    });
            string agentReply = string.Empty;
            var agent = base.GetAzureAgent(configuration["ProductAgentId"]);
            AgentThread thread = new AzureAIAgentThread(agent.Item2);         
            using var textSearchStore = new TextSearchStore<string>(vectorStore, collectionName: "calculation-formulas", vectorDimensions: 3072);
            var textSearchProvider = new TextSearchProvider(textSearchStore);      
            thread.AIContextProviders.Add(textSearchProvider);
            ChatMessageContent chatMessageContent = new(AuthorRole.User, message);           
            await foreach (ChatMessageContent response in agent.Item1.InvokeAsync(chatMessageContent, thread))
            {
                agentReply = agentReply + response.Content;
            }
            return agentReply;
        }
    }
    
}
