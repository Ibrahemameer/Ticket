using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhinoTicketingSystem.Models.db_a79800_ticket
{
    [Table("Tbl_ChartOfAccounts", Schema = "dbo")]
    public partial class TblChartOfAccount
    {
        [Key]
        [Column("Chart_ID")]
        [Required]
        public int ChartId { get; set; }

        [Column("Chart_Code")]
        public int? ChartCode { get; set; }

        [Column("Chart_Name")]
        public string ChartName { get; set; }

        public string ChartFullName { get; set; }

        [Column("Account_Level")]
        public int? AccountLevel { get; set; }

        public int? ParentAccount { get; set; }

        public TblChartOfAccount TblChartOfAccount1 { get; set; }

        [Column("Accoutn_Type")]
        public string AccoutnType { get; set; }

        public string Direction { get; set; }

        [Column("OB")]
        public decimal? Ob { get; set; }

        public DateTime? Date { get; set; }

        public bool? IsConnectedWithContact { get; set; }

        public bool? IsConnectedWithCostCenter { get; set; }

        public bool? IsConnectedWithBusinessType { get; set; }

        public bool? IsManualJournalNotAllowed { get; set; }

        public bool? IsConnectedWithProject { get; set; }

        public int? FinancialStatementClassificationGroupId { get; set; }

        public string AccountingSerial { get; set; }

        public ICollection<TblChartOfAccount> TblChartOfAccounts1 { get; set; }
    }
}