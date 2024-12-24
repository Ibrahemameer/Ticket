using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using RhinoTicketingSystem.Models.db_a79800_ticket;
using Task = System.Threading.Tasks.Task;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Web;
using RhinoTicketingSystem.Models;

namespace RhinoTicketingSystem.Components.Pages.Archieving
{
    public partial class UploadDocumentAttachment
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected db_a79800_ticketService db_a79800_ticketService { get; set; }

        [Parameter]
        public int DocumentId { get; set; }

        // For tracking uploaded files in memory
        private List<UploadFileInfo> uploadedFilesPath = new();

        // For displaying upload status messages
        private string uploadMessage = "";

        // Collection of existing attachments from database
        protected IEnumerable<TblDocumentAttachment> documentAttachments;

        // Form state flags
        protected bool errorVisible;
        protected bool hasChanges = false;
        protected bool canEdit = true;

        // Current attachment being created/edited
        protected TblDocumentAttachment tblDocumentAttachment;

        protected RadzenDataGrid<TblDocumentAttachment> grid0;

        protected override async Task OnInitializedAsync()
        {
            await LoadAttachments();
        }

        private async Task LoadAttachments()
        {
            var query = new Query
            {
                Filter = "DocumentId == @0",
                FilterParameters = new object[] { DocumentId }
            };
            documentAttachments = await db_a79800_ticketService.GetTblDocumentAttachments(query);
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        private async Task DownloadFile(TblDocumentAttachment attachment)
        {
            try
            {
                var navigationUri = $"api/FileDownload/document/{attachment.Id}";
                NavigationManager.NavigateTo(navigationUri, true);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }
        private async Task ConfirmDelete(TblDocumentAttachment attachment)
        {
            var confirmed = await DialogService.Confirm(
                $"Are you sure you want to delete {attachment.FileName}?",
                "Delete Attachment",
                new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    CloseDialogOnEsc = true,
                    CloseDialogOnOverlayClick = true
                });

            if (confirmed == true)
            {
                try
                {
                    // Delete physical file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        attachment.FilePath, attachment.FileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    // Delete database record
                    await db_a79800_ticketService.DeleteTblDocumentAttachment(attachment.Id);

                    // Refresh grid
                    await LoadAttachments();

                    NotificationService.Notify(NotificationSeverity.Success, "Success", "Attachment deleted successfully");
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
                }
            }
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            try
            {
                foreach (var file in e.GetMultipleFiles())
                {
                    var fileInfo = new UploadFileInfo
                    {
                        Filename = file.Name,
                        FileSize = file.Size
                    };

                    // Create document attachment record
                    var attachment = new TblDocumentAttachment
                    {
                        DocumentId = DocumentId,
                        FileName = file.Name,
                        FileSize = file.Size.ToString(),
                        FileType = Path.GetExtension(file.Name),
                        CreatedIn = DateTime.Now,
                        CreatedBy = DateTime.Now,
                        FilePath = Path.Combine("Uploads", "Documents", DocumentId.ToString())
                    };

                    // Save file physically
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FilePath);
                    Directory.CreateDirectory(path);

                    using (var stream = file.OpenReadStream())
                    {
                        using (var fs = File.Create(Path.Combine(path, file.Name)))
                        {
                            await stream.CopyToAsync(fs);
                        }
                    }

                    // Save to database
                    await db_a79800_ticketService.CreateTblDocumentAttachment(attachment);

                    uploadedFilesPath.Add(fileInfo);
                }

                await LoadAttachments();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }

        private async Task DeleteUploadedFile(UploadFileInfo file)
        {
            try
            {
                var attachment = documentAttachments.FirstOrDefault(a => a.FileName == file.Filename);
                if (attachment != null)
                {
                    await db_a79800_ticketService.DeleteTblDocumentAttachment(attachment.Id);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        "Uploads", "Documents", DocumentId.ToString(), file.Filename);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    uploadedFilesPath.Remove(file);
                    await LoadAttachments();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                // Set creation metadata
                tblDocumentAttachment.CreatedIn = DateTime.Now;
                tblDocumentAttachment.CreatedBy = DateTime.Now;

                await db_a79800_ticketService.CreateTblDocumentAttachment(tblDocumentAttachment);
                DialogService.Close(tblDocumentAttachment);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
                NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }
        private async Task GenerateSigningLink(TblDocumentAttachment attachment)
        {
            try
            {
                var signingSession = new SigningSession
                {
                    Token = Guid.NewGuid().ToString(),
                    AttachmentId = attachment.Id,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                await db_a79800_ticketService.CreateSigningSession(signingSession);

                var signingUrl = $"{NavigationManager.BaseUri}pdf-sign/{signingSession.Token}";
                await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", signingUrl);

                NotificationService.Notify(NotificationSeverity.Success,
                    "Success", "Signing link copied to clipboard");
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error,
                    "Error", "Could not generate signing link");
            }
        }


        public class UploadFileInfo
        {
            public string Filename { get; set; }
            public long FileSize { get; set; }
            public long UploadedBytes { get; set; }
            public double UploadedPercentage => (UploadedBytes * 100.0) / FileSize;
        }
    }

}
