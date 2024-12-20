using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class TestForFileUploadAndList
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

        protected bool errorVisible;
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicket;

        protected void CreateAttchement()
        {
            db_a79800_ticketService.CreateTicketAttachment(uploadedFilesPath);
        }
       public class FileAttchment
        {
            public int TicketId { get; set; }
            public string Filename { get; set; }
            public string FilePath { get; set; }
            public string FileSize { get; set; }
            public string FileType { get; set; }

            // New properties for upload tracking
            public long UploadedBytes { get; set; } // Track how many bytes have been uploaded
            public double UploadedPercentage { get; set; } // Calculate percentage
        }

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
