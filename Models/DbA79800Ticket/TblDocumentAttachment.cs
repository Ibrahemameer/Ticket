using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_DocumentAttachments", Schema = "dbo")]
    public partial class TblDocumentAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? DocumentId { get; set; }

        public TblDocumentHeder TblDocumentHeder { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileSize { get; set; }

        public DateTime? CreatedBy { get; set; }

        public DateTime? CreatedIn { get; set; }

        public string FileType { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

}