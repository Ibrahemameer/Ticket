using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using RhinoTicketingSystem.Controllers;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class EngineerProceedWithAssignedTicket
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
        [Inject]
        private UploadController upload { get; set; }
        protected override async Task OnInitializedAsync()
        {

            tblTicket = await db_a79800_ticketService.GetTblTicketByTicketId(TicketId);

            tblCategoriesForCategoryId = await db_a79800_ticketService.GetTblCategories();

            tblEngineersForEngineerId = await db_a79800_ticketService.GetTblEngineers();

            tblEmployeesForEmployeeId = await db_a79800_ticketService.GetTblEmployees();

            tblStatusesForStatusId = await db_a79800_ticketService.GetTblStatusesForProceeding();
        }
        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicket;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> tblCategoriesForCategoryId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> tblEngineersForEngineerId;

        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> tblEmployeesForEmployeeId;


        protected IEnumerable<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> tblStatusesForStatusId;

        protected async Task FormSubmit()
        {
            try
            {
                await db_a79800_ticketService.UpdateTblTicket(TicketId, tblTicket);
                DialogService.Close(tblTicket);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }
        protected async Task FormSubmitEngineerComment()
        {
            try
            {
                await db_a79800_ticketService.UpdateTblTicketWIthEngineerComment(Security.User.Email,TicketId, tblTicket);
                DialogService.Close(tblTicket);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }

        protected async Task FormSubmit2(string attachment)
        {
            try
            {

                await db_a79800_ticketService.UpdateTblTicketWithAttachment(attachment, TicketId, tblTicket);
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


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
            db_a79800_ticketService.Reset();
            hasChanges = false;
            canEdit = true;

            tblTicket = await db_a79800_ticketService.GetTblTicketByTicketId(TicketId);
        }

    }
}
