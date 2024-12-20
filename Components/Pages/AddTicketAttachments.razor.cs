using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using RhinoTicketingSystem.Controllers;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Forms;
using static RhinoTicketingSystem.Components.Pages.TestForFileUploadAndList;

namespace RhinoTicketingSystem.Components.Pages
{
    public partial class AddTicketAttachments
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected TooltipService TooltipService { get; set; }
        [Inject] protected ContextMenuService ContextMenuService { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }
        [Inject] public db_a79800_ticketService db_a79800_ticketService { get; set; }
        [Inject] protected SecurityService Security { get; set; }
        [Inject] private UploadController upload { get; set; }
        [Parameter] public int TicketId { get; set; }

        protected RadzenDataGrid<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment> grid0;
        protected IList<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment> tblTicketAttachments;
        protected List<FileAttchment> uploadedFilesPath = new List<FileAttchment>();
        protected RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket tblTicket;
        protected bool errorVisible;
        protected bool hasChanges = false;
        protected bool canEdit = true;
        protected string uploadMessage;

        private const long maxFileSizePerFile = 25 * 1024 * 1024; // 25 MB per file
        private const long maxTotalSize = 40 * 1024 * 1024; // 40 MB total

        protected override async Task OnInitializedAsync()
        {
            await GetChildData(TicketId);
            tblTicket = await db_a79800_ticketService.GetTblTicketByTicketId(TicketId);
        }

        protected async Task GetChildData(int ticketId)
        {
            tblTicketAttachments = await db_a79800_ticketService.GetTblTicketAttachments(TicketId);
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            uploadedFilesPath.Clear();
            foreach (var file in e.GetMultipleFiles())
            {
                var file1 = new FileAttchment
                {
                    Filename = file.Name,
                    FileSize = (file.Size / (1024.0 * 1024.0)).ToString("0.000") + " MB",
                    FileType = Path.GetExtension(file.Name).ToLowerInvariant(),
                    TicketId = tblTicket.TicketId,
                    UploadedBytes = 0
                };
                uploadedFilesPath.Add(file1);

                // Create full path including OneDrive folder
                var filePath = Path.Combine(env.WebRootPath, "Upload", "OneDrive - Albassami", file.Name);

                // Use FileStream with FileShare.ReadWrite to prevent file locking
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (var inputStream = file.OpenReadStream(maxFileSizePerFile))
                {
                    await inputStream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                    file1.UploadedBytes = fileStream.Length;
                    file1.UploadedPercentage = 100;
                }

                // Force a file system flush to ensure write completion
                System.IO.File.SetAttributes(filePath, System.IO.File.GetAttributes(filePath));
            }

            CreateAttchement();
            await RefreshAttachments();
            uploadMessage = $"Files uploaded successfully! Number Of Files Uploaded: {uploadedFilesPath.Count}";
        }
        private void CreateAttchement()
        {
            db_a79800_ticketService.CreateAttachmentForTicketByTicketId(uploadedFilesPath);
            grid0.Reload();
        }

        private void DeleteUploadedFile(FileAttchment attachment)
        {
            uploadedFilesPath.Remove(attachment);
            StateHasChanged();
        }

        protected async Task RefreshAttachments()
        {
            tblTicketAttachments = await db_a79800_ticketService.GetTblTicketAttachments(tblTicket.TicketId);
            await grid0.Reload();
        }

        private async Task ConfirmDeleteAttachmentFromDatabase(RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment attachment)
        {
            var result = await DialogService.Confirm("Are you sure you want to delete this attachment?", "Confirm Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (result.HasValue && result.Value)
            {
                await DeleteAttachmentFromDatabase(attachment);
            }
        }

        private async Task DeleteAttachmentFromDatabase(RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment attachment)
        {
            var filePath = Path.Combine(env.WebRootPath, "Upload", "OneDrive - Albassami", attachment.AttachedFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await db_a79800_ticketService.DeleteAttachmentAsync(attachment.Id);
            await RefreshAttachments();
        }

        private void DownloadFile(int id)
        {
            NavigationManager.NavigateTo($"api/filedownload/{id}", true);
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

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
            db_a79800_ticketService.Reset();
            hasChanges = false;
            canEdit = true;
            tblTicket = await db_a79800_ticketService.GetTblTicketByTicketId(TicketId);
        }
    }
}
