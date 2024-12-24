using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_Projects", Schema = "dbo")]
    public partial class TblProject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime? CreatedIn { get; set; }
        public string CreatedBy { get; set; }
        public string ProjectBranch { get; set; }
        public int? DocumentSerialId { get; set; }
        public string DocumentSerial { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<TblDocumentHeder> TblDocumentHeders { get; set; }
    }

}