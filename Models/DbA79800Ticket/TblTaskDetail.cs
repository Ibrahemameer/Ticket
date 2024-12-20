using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("TblTaskDetails", Schema = "dbo")]
    public partial class TblTaskDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        public int? TaskId { get; set; }

        public Task Task { get; set; }

        public int? EngineerId { get; set; }

        public TblEngineer TblEngineer { get; set; }

        public DateTime? ActionDate { get; set; }

        public int? TaskstatusId { get; set; }

        [Column("taskStatus")]
        public string TaskStatus { get; set; }

        public string EngineerComment { get; set; }
    }
}