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
    public partial class EditActiveTickets
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

        [Parameter]
        public int TicketId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            tblTicket = await db_a79800_ticketService.GetTblTicketByTicketId(TicketId);

            tblCategoriesForCategoryId = await db_a79800_ticketService.GetTblCategories();

            tblEngineersForEngineerId = await db_a79800_ticketService.GetTblEngineers();

            tblEmployeesForEmployeeId = await db_a79800_ticketService.GetTblEmployees();
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicket;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> tblCategoriesForCategoryId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> tblEmployeesForEmployeeId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await db_a79800_ticketService.UpdateTblTicket(TicketId, tblTicket);
                DialogService.Close(tblTicket);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}