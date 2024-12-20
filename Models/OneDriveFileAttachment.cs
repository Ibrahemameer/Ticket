using Microsoft.AspNetCore.Components.Forms;

namespace RhinoTicketingSystem.Models
{
    public class OneDriveFileAttachment
    {
        public IBrowserFile File { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public long UploadedBytes { get; set; }
        public double UploadedPercentage { get; set; }
        public bool IsValid { get; set; } = true;
        public byte[] FileContent { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsDeleted { get; set; }  // Track deletion status
        public string ContentType { get; set; }
        public DateTime LastModified { get; set; }

    }
}
