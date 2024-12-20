using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_Categories", Schema = "dbo")]
    public partial class TblCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CategoryID")]
        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string Notes1 { get; set; }

        public string Notes2 { get; set; }

        public ICollection<TblTicket> TblTickets { get; set; }
    }
}