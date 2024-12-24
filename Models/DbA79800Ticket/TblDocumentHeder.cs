using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_DocumentHeder", Schema = "dbo")]
    public partial class TblDocumentHeder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DocumentSerial { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public int? ProjectId { get; set; }
        public TblProject TblProject { get; set; }
        public string Project { get; set; }
        public DateTime? Createdin { get; set; }
        public string CreatedBy { get; set; }

        // Add a single concurrency token instead of multiple checks
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<TblDocumentAttachment> TblDocumentAttachments { get; set; }
    }

}