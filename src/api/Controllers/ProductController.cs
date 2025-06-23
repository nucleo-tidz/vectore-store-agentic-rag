using api.Models;
using infrastructure.Agents;
using infrastructure.Repository;
using infrastructure.vector;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductRepository productRepository, Kernel kernel, IProjectAgent projectAgent) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] Models.Product product)
        {
            var vectorService = kernel.Services.GetRequiredService<IVectorService>();
            var productModel = product.ToModel();
            int result = await productRepository.Save(productModel);
            await vectorService.SaveAsync(productModel);
            return CreatedAtAction(nameof(Post), new { id = result }, product);
        }

        [HttpGet("/{description}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Post(string description)
        {
            var vectorService = kernel.Services.GetRequiredService<IVectorService>();
            var suggestion = await vectorService.Search(description);
            return Ok(suggestion);
        }
        [HttpGet("chat/{message}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message)
        {
           var response=await projectAgent.Execute(message);
            return Ok(response);
        }
    }
}
