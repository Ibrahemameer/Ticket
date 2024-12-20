using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tasks", Schema = "dbo")]
    public partial class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        [Required]
        public int EngineerId { get; set; }

        public TblEngineer TblEngineer { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int TypeId { get; set; }

        public TaskType TaskType { get; set; }

        [Required]
        public int StatusId { get; set; }

        public TaskStatus TaskStatus { get; set; }

        public ICollection<TblTaskDetail> TblTaskDetails { get; set; }
    }
}