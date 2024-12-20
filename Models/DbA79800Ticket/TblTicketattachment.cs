using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RhinoTicketingSystem.Models.db_a79800_ticket;

namespace RhinoTicketingSystem.Models.DbA79800Ticket
{
    [Table("Tbl_Ticketattachments", Schema = "dbo")]
    public class TblTicketattachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int Id { get; set; }
        public int TicketId { get; set; }

        public string AttachedFileName { get; set; }

        public string AttachedFilePath { get; set; }

        public string attachedFileSize { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FileType { get; set; }
        public TblTicket tblTickets { get; set; }
    }
}
