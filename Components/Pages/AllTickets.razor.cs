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
    public partial class AllTickets
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

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> tblTickets;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            tblTickets = await db_a79800_ticketService.GetTblTickets(new Query { Filter = $@"i => i.TicketNumber.Contains(@0) || i.TicketHeader.Contains(@0) || i.TicketDescription.Contains(@0) || i.UserEmail.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblCategory,TblEngineer,TblEmployee" });
        }
        protected override async Task OnInitializedAsync()
        {
            tblTickets = await db_a79800_ticketService.GetTblTickets(new Query { Filter = $@"i => i.TicketNumber.Contains(@0) || i.TicketHeader.Contains(@0) || i.TicketDescription.Contains(@0) || i.UserEmail.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblCategory,TblEngineer,TblEmployee" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddAllTickets>("Add TblTicket", null);
            await grid0.Reload();
        }

        protected async Task EditRow(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            await DialogService.OpenAsync<EditAllTickets>("Edit TblTicket", new Dictionary<string, object> { { "TicketId", args.TicketId } });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicket)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await db_a79800_ticketService.DeleteTblTicket(tblTicket.TicketId);

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
                    Detail = $"Unable to delete TblTicket"
                });
            }
        }
        protected async Task OpenAttachmentForm(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            await DialogService.OpenAsync<AddTicketAttachments>("Add ticket attachment",
                new Dictionary<string, object> { { "TicketId", args.TicketId } },
                new DialogOptions
                {
                    Width = "80vw",
                    Height = "80vh",
                    Left = "10vw",
                    Top = "10vh",
                    Resizable = true,
                    Draggable = true,
                    Style = "min-width: 800px; max-width: 1200px; margin: 0 auto;"
                });
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await db_a79800_ticketService.ExportTblTicketsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "TblCategory,TblEngineer,TblEmployee",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblTickets");
            }

            if (args == null || args.Value == "xlsx")
            {
                await db_a79800_ticketService.ExportTblTicketsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "TblCategory,TblEngineer,TblEmployee",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblTickets");
            }
        }
    }
}