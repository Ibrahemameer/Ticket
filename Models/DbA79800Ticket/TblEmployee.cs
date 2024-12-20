using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_Employees", Schema = "dbo")]
    public partial class TblEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        public int? PersonnelNumber { get; set; }

        public string Name { get; set; }

        public string Sector { get; set; }

        public string Site { get; set; }

        public string Center { get; set; }

        public string Email { get; set; }

        public ICollection<TblTicket> TblTickets { get; set; }
    }
}