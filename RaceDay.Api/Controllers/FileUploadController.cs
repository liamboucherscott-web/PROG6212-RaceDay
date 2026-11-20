using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FileUploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Uploads an image file to Azure Blob Storage and returns its public URL.
       
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var connectionString = _configuration["AzureBlob:ConnectionString"];
            var containerName = _configuration["AzureBlob:ContainerName"];

            // Guard: if no storage key is configured 
            // return a clear message instead of crashing.
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return StatusCode(503, new { error = "Azure Blob Storage is not configured on this environment." });
            }

            var containerClient = new BlobContainerClient(connectionString, containerName);

            try
            {
                var blobName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return Ok(new { url = blobClient.Uri.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Upload failed: " + ex.Message });
            }
        }
    }
}