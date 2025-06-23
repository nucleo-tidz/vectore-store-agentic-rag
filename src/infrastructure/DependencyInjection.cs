using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.Diagnostics.CodeAnalysis;

namespace infrastructure
{
    [Experimental("SEMX")]
    public static class DependencyInjection
    {
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
                   "o4-mini")
                .AddRedisVectorStore("");
                return kernelBuilder.Build();
            });
        }
    }
}
