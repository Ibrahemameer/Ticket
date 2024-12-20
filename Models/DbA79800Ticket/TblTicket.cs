using RhinoTicketingSystem.Models.DbA79800Ticket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_Tickets", Schema = "dbo")]
    public partial class TblTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TicketID")]
        public int TicketId { get; set; }

        public string TicketNumber { get; set; }

        public string TicketHeader { get; set; }

        public string TicketDescription { get; set; }
        public string EngEmail { get; set; }
        public string Attachment { get; set; }//
        public string AttchedFileName { get; set; }//
        public Int64? AttchedFileSize { get; set; }
        public string EngineerComment { get; set; }
        public string TicketStatus { get; set; }

        
        public int? StatusId { get; set; }
        public DateTime? Date { get; set; }

        [Column("CategoryID")]
        public int? CategoryId { get; set; }

        public TblCategory TblCategory { get; set; }

        public int? EngineerId { get; set; }

        public TblEngineer TblEngineer { get; set; }

        public int? EmployeeId { get; set; }

        public TblEmployee TblEmployee { get; set; }

        public string UserEmail { get; set; }

        public ICollection<TblReassignTicket> TblReassignTickets { get; set; }
        public ICollection<TblTicketattachment> TblTicketattachments { get; set; }
    }
}