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
    public partial class MyAssignedTickets
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

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            tblTickets = await db_a79800_ticketService.GetTblTicketsForEngineer(Security.User.Email,new Query { Filter = $@"i => i.TicketNumber.Contains(@0) || i.TicketHeader.Contains(@0) || i.TicketDescription.Contains(@0) || i.UserEmail.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblCategory,TblEngineer,TblEmployee" });
        }

            protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> tblCategoriesForCategoryId;

            protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerId;

            protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> tblEmployeesForEmployeeId;
        protected override async Task OnInitializedAsync()
        {
            tblTickets = await db_a79800_ticketService.GetTblTicketsForEngineer(Security.User.Email, new Query { Filter = $@"i => i.TicketNumber.Contains(@0) || i.TicketHeader.Contains(@0) || i.TicketDescription.Contains(@0) || i.UserEmail.Contains(@0)", FilterParameters = new object[] { search }, Expand = "TblCategory,TblEngineer,TblEmployee" });

            tblCategoriesForCategoryId = await db_a79800_ticketService.GetTblCategories();

            tblEngineersForEngineerId = await db_a79800_ticketService.GetTblEngineers();

            tblEmployeesForEmployeeId = await db_a79800_ticketService.GetTblEmployees();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket());
        }
        protected async Task OpenAttachmentForm(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            await DialogService.OpenAsync<AddTicketAttachments>("Add ticket attachment to my ticket", new Dictionary<string, object> { { "TicketId", args.TicketId } });
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

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await db_a79800_ticketService.ExportTblTicketsToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "TblCategory,TblEngineer,TblEmployee",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "TblTickets");
            }

            if (args == null || args.Value == "xlsx")
            {
                await db_a79800_ticketService.ExportTblTicketsToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "TblCategory,TblEngineer,TblEmployee",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "TblTickets");
            }
        }

        protected async Task GridRowUpdate(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            try
            {
                await db_a79800_ticketService.UpdateTblTicket(args.TicketId, args);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                      Severity = NotificationSeverity.Error,
                      Summary = $"Error",
                      Detail = $"Unable to update TblTicket"
                });
            }
        }

        protected async Task GridRowCreate(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            try
            {
                await db_a79800_ticketService.CreateTblTicket(args);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                      Severity = NotificationSeverity.Error,
                      Summary = $"Error",
                      Detail = $"Unable to create TblTicket"
                });
            }
            await grid0.Reload();
        }
        protected async Task OpenTaskPageForEngineer(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            await DialogService.OpenAsync<EngineerProceedWithAssignedTicket>("Ticket Proceed", new Dictionary<string, object> { { "TicketId", args.TicketId } });
        }
        protected async Task EditButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket data)
        {
            await grid0.EditRow(data);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket data)
        {
            grid0.CancelEditRow(data);
            await db_a79800_ticketService.CancelTblTicketChanges(data);
        }

        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicketChild;
        protected async Task GetChildData(RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket args)
        {
            tblTicketChild = args;
            var TblTicketAttachmentsResult = await db_a79800_ticketService.GetTblReassignTickets(new Query { Filter = $@"i => i.TicketId == {args.TicketId}", Expand = "TblTicket,TblEngineer,TblStatus" });
            if (TblTicketAttachmentsResult != null)
            {
                args.TblReassignTickets = TblTicketAttachmentsResult.ToList();
            }
        }
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket tblReassignTicketTblReassignTickets;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> tblTicketsForTicketIdTblReassignTickets;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerIdTblReassignTickets;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> tblStatusesForStatusIdTblReassignTickets;

        protected RadzenDataGrid<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> TblReassignTicketsDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task TblReassignTicketsAddButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket data)
        {

            await TblReassignTicketsDataGrid.InsertRow(new RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket());

        }

        protected async Task TblReassignTicketsCancelButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket data)
        {
            TblReassignTicketsDataGrid.CancelEditRow(data);
            await db_a79800_ticketService.CancelTblReassignTicketChanges(data);
            await TblReassignTicketsDataGrid.Reload();
        }

        protected async Task TblReassignTicketsSaveButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket data)
        {
            await TblReassignTicketsDataGrid.UpdateRow(data);
            await db_a79800_ticketService.UpdateTblReassignTicket(data.Id, data);
            await TblReassignTicketsDataGrid.Reload();
        }

        protected async Task TblReassignTicketsEditButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket data)
        {

            tblTicketsForTicketIdTblReassignTickets = await db_a79800_ticketService.GetTblTickets();

            tblEngineersForEngineerIdTblReassignTickets = await db_a79800_ticketService.GetTblEngineers();

            tblStatusesForStatusIdTblReassignTickets = await db_a79800_ticketService.GetTblStatuses();

            await TblReassignTicketsDataGrid.EditRow(data);
        }

        protected async Task TblReassignTicketsDeleteButtonClick(MouseEventArgs args, RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket tblReassignTicket)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await db_a79800_ticketService.DeleteTblReassignTicket(tblReassignTicket.Id);

                    await GetChildData(tblTicketChild);

                    if (deleteResult != null)
                    {
                        await TblReassignTicketsDataGrid.Reload();
                    }
                }
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete TblReassignTicket"
                });
            }
        }
    }
}