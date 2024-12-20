using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.DbA79800Ticket
{
    public class OneDriveArchivingDetail
    {
        [Key]
        public int ArchiveDetailId { get; set; }
        public int ArchiveId { get; set; }
        public string AttachedFileName { get; set; }
        public string AttachedFilePath { get; set; }
        public string FileType { get; set; }
        public string AttachedFileSize { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        [ForeignKey("ArchiveId")]
        public virtual OneDriveArchivingHeader ArchiveHeader { get; set; }
    }
}
