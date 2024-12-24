using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class TblDocumentSerializes
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public db_a79800_ticketService db_a79800_ticketService { get; set; }

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentSerialize> tblDocumentSerializes;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentSerialize> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            tblDocumentSerializes = await db_a79800_ticketService.GetTblDocumentSerializes(new Query { Filter = $@"i => i.FirstSerial.Contains(@0) || i.SecondSerial.Contains(@0) || i.ThirdSerial.Contains(@0) || i.FourthSerial.Contains(@0) || i.Combination.Contains(@0) || i.DocumentType.Contains(@0) || i.CreatedBy.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblDepartment" });
        }
        protected override async Task OnInitializedAsync()
        {
            tblDocumentSerializes = await db_a79800_ticketService.GetTblDocumentSerializes(new Query { Filter = $@"i => i.FirstSerial.Contains(@0) || i.SecondSerial.Contains(@0) || i.ThirdSerial.Contains(@0) || i.FourthSerial.Contains(@0) || i.Combination.Contains(@0) || i.DocumentType.Contains(@0) || i.CreatedBy.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblDepartment" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddTblDocumentSerialize>("Add TblDocumentSerialize", null);
            await grid0.Reload();
        }

        protected async Task EditRow(RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentSerialize args)
        {
            await DialogService.OpenAsync<EditTblDocumentSerialize>("Edit TblDocumentSerialize", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblDocumentSerialize tblDocumentSerialize)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await db_a79800_ticketService.DeleteTblDocumentSerialize(tblDocumentSerialize.Id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete TblDocumentSerialize"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await db_a79800_ticketService.ExportTblDocumentSerializesToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "TblDepartment",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblDocumentSerializes");
            }

            if (args == null || args.Value == "xlsx")
            {
                await db_a79800_ticketService.ExportTblDocumentSerializesToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "TblDepartment",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblDocumentSerializes");
            }
        }
        
    }
}