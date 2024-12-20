using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO.Pipes;
using RhinoTicketingSystem.Models.db_a79800_ticket;
using Microsoft.AspNetCore.Components;

namespace RhinoTicketingSystem.Controllers
{
    public partial class UploadController : Controller
    {
        public readonly IWebHostEnvironment environment;

        public UploadController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        // Single file upload
        [HttpPost("upload/single")]
        public IActionResult Single(IFormFile file)
        {
            try
            {
                // Put your code here
                
                UploadFile(file);   
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload
        [HttpPost("upload/multiple")]
        public IActionResult Multiple(IFormFile[] files)
        {
            try
            {
                
                // Put your code here
                foreach (var file in files)
                {
                    UploadFile(file);
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload with parameter
        [HttpPost("upload/{id}")]
        public IActionResult Post(IFormFile[] files, int id)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Image file upload (used by HtmlEditor components)
        [HttpPost("upload/image")]
        public IActionResult Image(IFormFile file)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(environment.WebRootPath, fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = Url.Content($"~/{fileName}");

                    return Ok(new { Url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        public RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicket { get; set; }
        [Parameter]
        public string fullPath { get; set; }
        public async System.Threading.Tasks.Task UploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var imagePath = @"\Upload";
                var uploadPath = environment.WebRootPath + imagePath;
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);

                }
                fullPath = Path.Combine(uploadPath, file.FileName);
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

        }
    }
}
