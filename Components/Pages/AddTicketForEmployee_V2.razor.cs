using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class AddTicketForEmployee_V2
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

        protected override async Task OnInitializedAsync()
        {
            tblTicket = new RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket();

            tblCategoriesForCategoryId = await db_a79800_ticketService.GetTblCategories();

            tblEngineersForEngineerId = await db_a79800_ticketService.GetTblEngineers();

            tblEmployeesForEmployeeId = await db_a79800_ticketService.GetTblEmployees();
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicket;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> tblCategoriesForCategoryId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> tblEmployeesForEmployeeId;

        protected async Task FormSubmit()
        {
            try
            {
                await db_a79800_ticketService.CreateTblTicket(tblTicket);
                DialogService.Close(tblTicket);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }
        protected async Task FormSubmit2(string userName)
        {
            try
            {
                await db_a79800_ticketService.CreateTblTicket(tblTicket);



                await db_a79800_ticketService.CreateTicket_WithUpdatingUserEmail(userName, tblTicket.TicketId, tblTicket);
                DialogService.Close(tblTicket);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }
        protected async Task FormSubmit2WithAttachment(string filePath, string userName)
        {
            try
            {
                await db_a79800_ticketService.CreateTblTicket(tblTicket);



                await db_a79800_ticketService.CreateTicket_WithUpdatingUserEmailandAttachment(filePath, userName, tblTicket.TicketId, tblTicket);
                DialogService.Close(tblTicket);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        //thi sector is for uploading file for the ticket from the employee
        int progress;
        string info;

        void OnProgress(UploadProgressArgs args, string name)
        {
            this.info = $"% '{name}' / {args.Loaded} of {args.Total} bytes.";
            this.progress = args.Progress;
        }

        protected bool hasChanges = false;
        protected bool canEdit = true;

        [Inject]
        protected SecurityService Security { get; set; }

    }
}
