using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_Engineers", Schema = "dbo")]
    public partial class TblEngineer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        public int? EngineerCode { get; set; }

        public string Name { get; set; }

        public string EngEmail { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public ICollection<TblReassignTicket> TblReassignTickets { get; set; }

        public ICollection<TblTicket> TblTickets { get; set; }

        public ICollection<TblTaskDetail> TblTaskDetails { get; set; }
    }
}