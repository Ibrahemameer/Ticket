using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_Department", Schema = "dbo")]
    public partial class TblDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ConcurrencyCheck]
        public string DepartmentName { get; set; }

        [ConcurrencyCheck]
        public string ArchiveResponsible { get; set; }

        [ConcurrencyCheck]
        public DateTime? CreatedIn { get; set; }

        [ConcurrencyCheck]
        public string CreatedBy { get; set; }

        public ICollection<TblDocumentSerialize> TblDocumentSerializes { get; set; }
    }
}