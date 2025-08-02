using infrastructure.Agents;
using infrastructure.Repository;
using infrastructure.vector;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.Diagnostics.CodeAnalysis;

namespace infrastructure
{
    [Experimental("SEMX")]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IProductRepository, ProductRepository>()
                .AddSemanticKernel(configuration).
                AddTransient<IProjectAgent, ProjectAgent>();               
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
                  configuration["foundry-endpoint-mini"],
                  configuration["apikey-mini"],
                   "o4-mini",
                   "o4-mini");
               
                kernelBuilder.Services.AddRedisVectorStore(configuration.GetConnectionString("redis"),new()
                {
                    EmbeddingGenerator= kernelBuilder.Services.BuildServiceProvider().GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>(),
                });
                kernelBuilder.Services.AddTransient<IVectorService, VectorService>();
                kernelBuilder.Services.AddTransient<IDocumentService, DocumentService>();
                return kernelBuilder.Build();
            });
        }
    }
}
