using System.Diagnostics.CodeAnalysis;

using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.Identity;

using infrastructure.Agents;
using infrastructure.vector;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace infrastructure
{
    [Experimental("SEMX")]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSemanticKernel(configuration).
                AddTransient<IProjectAgent, ProjectAgent>().AddAgent(configuration);               
               return services;
        }
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddTransient<Kernel>(serviceProvider =>
            {
                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            
                kernelBuilder.AddAzureOpenAIEmbeddingGenerator
                (deploymentName: "text-embedding-3-large", endpoint: configuration["foundry-endpoint-embedder"],
                apiKey: configuration["apikey-embedder"]);

                kernelBuilder.Services.AddAzureOpenAIChatCompletion("o4-mini",
                 configuration["o4-mini-url"],
                 configuration["o4-mini-key"],
                  "o4-mini",
                  "o4-mini");

                kernelBuilder.Services.AddRedisVectorStore(configuration.GetConnectionString("redis"),new()
                {
                    EmbeddingGenerator= kernelBuilder.Services.BuildServiceProvider().GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>(),
                });        
                kernelBuilder.Services.AddTransient<IDocumentService, DocumentService>();
                return kernelBuilder.Build();
            });
        }
        public static IServiceCollection AddAgent(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(_ =>
            {
                var credential = new DefaultAzureCredential(
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

                });
                var projectClient = new AIProjectClient(new Uri(configuration["AgentProjectEndpoint"]), credential);
                return projectClient.GetPersistentAgentsClient();
            });
            return services;
        }
    }
}
