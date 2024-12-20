using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_Status", Schema = "dbo")]
    public partial class TblStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatusId { get; set; }

        public string Description { get; set; }

        public ICollection<TblReassignTicket> TblReassignTickets { get; set; }
    }
}