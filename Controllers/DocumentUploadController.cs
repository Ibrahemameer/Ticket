using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;

namespace RhinoTicketingSystem.Controllers
{
    [Route("document-upload")]
    public class DocumentUploadController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;
        public DocumentUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        private string GetContentType(string fileName)
        {
            const string DefaultContentType = "application/octet-stream";

            if (!_contentTypeProvider.TryGetContentType(fileName, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }

        [HttpPost("single")]
        public async Task<IActionResult> Single(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var path = Path.Combine(_environment.WebRootPath, "uploads", file.FileName);
            var contentType = GetContentType(file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { FileName = file.FileName, FileType = contentType, FileSize = file.Length });
        }
        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var path = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(path), Path.GetFileName(fileName));
        }

    }
}
