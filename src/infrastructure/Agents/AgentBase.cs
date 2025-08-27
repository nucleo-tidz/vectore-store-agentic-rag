using System.Diagnostics.CodeAnalysis;

using Azure.AI.Agents.Persistent;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;

namespace infrastructure.Agents
{
    [Experimental("SKEXP0110")]
    public class AgentBase(Kernel kernel, PersistentAgentsClient agentsClient)
    {
        public (Agent, PersistentAgentsClient) GetAzureAgent(string agentId)
        {
            PersistentAgent definition = agentsClient.Administration.GetAgent(agentId);
            AzureAIAgent agent = new(definition, agentsClient)
            {
                Kernel = kernel,
            };
            return (agent, agentsClient);
        }
        public async Task UpdateAgent(string agentId, object schema)
        {
            await agentsClient.Administration.UpdateAgentAsync(agentId, responseFormat: BinaryData.FromObjectAsJson(schema));
        }
    }
}