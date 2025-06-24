﻿using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using System.Diagnostics.CodeAnalysis;

namespace infrastructure.Agents
{
    [Experimental("SKEXP0110")]
    public class AgentBase(Kernel kernel, IConfiguration configuration)
    {
        public (Agent, PersistentAgentsClient) GetAzureAgent(string agentId)
        {
            //DO NO USE THIS CODE IN PRODUCTION. USE RBAC INSTEAD FOR AUTHENTICATION
            AIProjectClient projectClient = new AIProjectClient(new Uri(configuration["AgentProjectEndpoint"]), new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    ExcludeVisualStudioCredential = true,
                    ExcludeEnvironmentCredential = true,
                    ExcludeManagedIdentityCredential = true,
                    ExcludeInteractiveBrowserCredential = false,
                    ExcludeAzureCliCredential = false,
                    ExcludeAzureDeveloperCliCredential = true,
                    ExcludeAzurePowerShellCredential = true,
                    ExcludeSharedTokenCacheCredential = true,
                    ExcludeVisualStudioCodeCredential = true,
                    ExcludeWorkloadIdentityCredential = true,

                }));

            PersistentAgentsClient agentsClient = projectClient.GetPersistentAgentsClient();
            PersistentAgent definition = agentsClient.Administration.GetAgent(agentId);
            AzureAIAgent agent = new(definition, agentsClient)
            {
                Kernel = kernel,
            };
            return (agent, agentsClient);
        }
    }
}