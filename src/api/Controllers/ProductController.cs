

using infrastructure.Agents;
using infrastructure.vector;

using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController( Kernel kernel, IProjectAgent projectAgent) : ControllerBase
    {
       
        [HttpGet("chat/{message}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message)
        {
            var response = await projectAgent.Execute(message);
            return Ok(response);
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var documentService = kernel.Services.GetRequiredService<IDocumentService>();
            string fileContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = await reader.ReadToEndAsync();
            }
            await documentService.SaveAsync(fileContent);
            return Ok("File uploaded successfully.");
        }
    }
}
