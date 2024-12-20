using System.ComponentModel.DataAnnotations;

namespace RhinoTicketingSystem.Models.DbA79800Ticket
{
    public class OneDriveArchivingHeader
    {
        [Key]
        public int ArchiveId { get; set; }
        public string FolderName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public virtual ICollection<OneDriveArchivingDetail> ArchiveDetails { get; set; }
    }
}
