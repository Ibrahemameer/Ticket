using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_ReassignTicket", Schema = "dbo")]
    public partial class TblReassignTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        public int? TicketId { get; set; }

        public TblTicket TblTicket { get; set; }

        public int? EngineerId { get; set; }

        public TblEngineer TblEngineer { get; set; }

        public string ReassignedBy { get; set; }
        public string ReassignedTo { get; set; }

        public DateTime? ReassignedDate { get; set; }

        public string ProblemDescription { get; set; }

        public int? StatusId { get; set; }

        public TblStatus TblStatus { get; set; }

        public string UserEmail { get; set; }
    }
}