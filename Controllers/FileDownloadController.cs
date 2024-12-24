using Microsoft.AspNetCore.Mvc;
using RhinoTicketingSystem.Data;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;


namespace RhinoTicketingSystem.Controllers
{
    [Route("api/[controller]")]
    public class FileDownloadController : ControllerBase
    {
        private readonly db_a79800_ticketContext _context;

        public FileDownloadController(db_a79800_ticketContext context)
        {
            _context = context;
        }
        //[HttpGet("{ticketId}")]
        //public IActionResult DownloadFile(int ticketId)
        //{

        //    var ticket = _context.TblTickets.FirstOrDefault(t => t.TicketId == ticketId);
        //    if (ticket == null || string.IsNullOrEmpty(ticket.AttchedFileName))
        //    {
        //        return NotFound();
        //    }

        //    var filePath = Path.Combine("wwwroot", "Upload", ticket.AttchedFileName);
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound();
        //    }

        //    var fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    var contentTypeProvider = new FileExtensionContentTypeProvider();
        //    string contentType;

        //    // Try to get the content type based on the file extension
        //    if (!contentTypeProvider.TryGetContentType(ticket.AttchedFileName, out contentType))
        //    {
        //        contentType = "application/octet-stream"; // Default if not found
        //    }

        //    var fileName = Path.GetFileName(filePath);
        //    return File(fileBytes, contentType, fileName);
        //}
        [HttpGet("document/{attachmentId}")]
        public IActionResult DownloadDocumentFile(int attachmentId)
        {
            var attachment = _context.TblDocumentAttachments.FirstOrDefault(t => t.Id == attachmentId);
            if (attachment == null || string.IsNullOrEmpty(attachment.FileName))
            {
                return NotFound();
            }

            var filePath = Path.Combine("wwwroot", attachment.FilePath, attachment.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!contentTypeProvider.TryGetContentType(attachment.FileName, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(fileBytes, contentType, attachment.FileName);
        }
        [HttpGet("view/{id}")]
        public IActionResult ViewPdf(int id)
        {
            var attachment = _context.TblDocumentAttachments.FirstOrDefault(t => t.Id == id);
            if (attachment == null)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                attachment.FilePath, attachment.FileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            // Using Append instead of Add
            Response.Headers.Append("Content-Disposition", "inline; filename=" + attachment.FileName);
            return File(System.IO.File.ReadAllBytes(filePath), "application/pdf");
        }



        [HttpGet("{ticketId}")]
        public IActionResult DownloadFile(int ticketId)
        {

            var ticket = _context.TblTicketattachments.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null || string.IsNullOrEmpty(ticket.AttachedFileName))
            {
                return NotFound();
            }

            var filePath = Path.Combine("wwwroot", "Upload", "OneDrive - Albassami", ticket.AttachedFileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            string contentType;

            // Try to get the content type based on the file extension
            if (!contentTypeProvider.TryGetContentType(ticket.AttachedFileName, out contentType))
            {
                contentType = "application/octet-stream"; // Default if not found
            }

            var fileName = Path.GetFileName(filePath);
            return File(fileBytes, contentType, fileName);
        }
    }
}
