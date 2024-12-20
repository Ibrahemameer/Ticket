using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using System.Collections.Concurrent;
using DocumentFormat.OpenXml.InkML;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class TblEngineers
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

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineers;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> grid0;

        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer tblEngineer;
        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> reAssignedTickets;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            tblEngineers = await db_a79800_ticketService.GetTblEngineers(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            tblEngineers = await db_a79800_ticketService.GetTblEngineers(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } });
            //ticketsForEngineer = await db_a79800_ticketService.GetTblReassignTicketByEngineerId(engineerId);

            
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddTblEngineer>("Add TblEngineer", null);
            await grid0.Reload();
        }
        //protected async Task GetTblReassignTicketByEngineerId(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer tblEngineer)
        //{
        //    await db_a79800_ticketService.GetTblReassignTicketByEngineerId(tblEngineer.Id);
        //}
        protected async Task EditRow(RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer args)
        {
            await DialogService.OpenAsync<EditTblEngineer>("Edit TblEngineer", new Dictionary<string, object> { { "Id", args.Id } });
        }
        public int engineerId;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket ticketsForEngineer;

        protected async Task GridDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer tblEngineer)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {

                    //var deleteResult = await db_a79800_ticketService.DeleteTblEngineer(tblEngineer.Id);
                    var deleteResult = await db_a79800_ticketService.DeleteEngineerAfterCheckForHisTickets(tblEngineer.Id);
                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            
            catch (ApplicationException ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    
                    Severity = NotificationSeverity.Warning,
                    Summary = $"Delete operation is not allowed",
                    Detail = $"This Engineer has Related Assigned Tickets : Unable to delete"
                });
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete TblEngineer"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await db_a79800_ticketService.ExportTblEngineersToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblEngineers");
            }

            if (args == null || args.Value == "xlsx")
            {
                await db_a79800_ticketService.ExportTblEngineersToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "TblEngineers");
            }
        }
    }
}