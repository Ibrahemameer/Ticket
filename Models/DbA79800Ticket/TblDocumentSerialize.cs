using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_DocumentSerialize", Schema = "dbo")]
    public partial class TblDocumentSerialize
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ConcurrencyCheck]
        public string FirstSerial { get; set; }

        [ConcurrencyCheck]
        public string SecondSerial { get; set; }

        [ConcurrencyCheck]
        public string ThirdSerial { get; set; }

        [ConcurrencyCheck]
        public string FourthSerial { get; set; }

        [ConcurrencyCheck]
        public string Combination { get; set; }

        [ConcurrencyCheck]
        public string DocumentType { get; set; }

        [ConcurrencyCheck]
        public int? DepartmentId { get; set; }

        public TblDepartment TblDepartment { get; set; }

        [ConcurrencyCheck]
        public DateTime? CreatedIn { get; set; }

        [ConcurrencyCheck]
        public string CreatedBy { get; set; }
    }
}