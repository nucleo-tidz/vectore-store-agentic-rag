

using infrastructure.Agents;
using infrastructure.vector;

using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController( Kernel kernel, IProjectAgent projectAgent) : ControllerBase
    {

        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [HttpGet("chat/{message}/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message, string username, string? threadId)
        {
            //A container scheduled for dispatch has a gross weight of 9500 kg and a volume of 8.3 CBM. It is designated for a FuelSensitive route and requires special handling due to the presence of hazardous materials. Please calculate the total shipping cost using a base rate of ₹1001 per CBM.
             var response = await projectAgent.Execute(message, username, threadId);
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
