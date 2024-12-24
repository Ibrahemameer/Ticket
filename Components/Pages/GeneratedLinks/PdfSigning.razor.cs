using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using RhinoTicketingSystem.Models.db_a79800_ticket;
using RhinoTicketingSystem.Models;
using Task = System.Threading.Tasks.Task;
using RhinoTicketingSystem.Services;
using FileInfo = System.IO.FileInfo;

namespace RhinoTicketingSystem.Components.Pages.GeneratedLinks
{
    public partial class PdfSigning
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
        public string Token { get; set; }

        private string pdfUrl;
        private RadzenSignaturePad signaturePad;
        private SigningSession signingSession;
        private TblDocumentAttachment attachment;
        private ElementReference signaturePadElement;
        protected override async Task OnInitializedAsync()
        {
            signingSession = await db_a79800_ticketService.GetSigningSessionByToken(Token);
            if (signingSession != null && !signingSession.IsUsed)
            {
                attachment = await db_a79800_ticketService.GetTblDocumentAttachmentById(signingSession.AttachmentId);
                if (attachment != null)
                {
                    // Use view endpoint instead of download
                    pdfUrl = $"api/FileDownload/view/{attachment.Id}";
                }
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeSignaturePad");
            }
        }

        private async Task ClearSignature()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("clearSignaturePad");
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Could not clear signature");
            }
        }

        private async Task ApplySignature()
        {
            try
            {
                // Validate session and attachment
                if (attachment == null || signingSession == null)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Invalid signature session");
                    return;
                }

                if (signingSession.IsUsed)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "This signing link has already been used");
                    return;
                }

                if (signingSession.ExpiresAt < DateTime.UtcNow)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "This signing link has expired");
                    return;
                }

                // Get signature data
                var signatureData = await JSRuntime.InvokeAsync<string>(
                    "eval",
                    "document.querySelector('#signaturePad canvas').toDataURL('image/png')"
                );

                if (string.IsNullOrEmpty(signatureData))
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Warning", "Please add a signature first");
                    return;
                }

                // Process signature data
                var base64Data = signatureData.Replace("data:image/png;base64,", "");
                var signatureBytes = Convert.FromBase64String(base64Data);

                // Create paths for files
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FilePath);
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var signedFileName = $"signed_{timestamp}_{attachment.FileName}";
                var signedPath = Path.Combine(basePath, signedFileName);

                // Apply signature to PDF
                var pdfService = new PdfSignatureService();
                await pdfService.ApplySignatureToPdf(
                    Path.Combine(basePath, attachment.FileName),
                    signedPath,
                    signatureBytes,
                    100, 100
                );

                // Create new attachment record
                var signedAttachment = new TblDocumentAttachment
                {
                    DocumentId = attachment.DocumentId,
                    FileName = signedFileName,
                    FilePath = attachment.FilePath,
                    FileSize = new FileInfo(signedPath).Length.ToString(),
                    FileType = ".pdf",
                    CreatedIn = DateTime.Now,
                    CreatedBy = DateTime.Now
                };

                await db_a79800_ticketService.CreateTblDocumentAttachment(signedAttachment);

                // Update signing session
                signingSession.IsUsed = true;
                await db_a79800_ticketService.UpdateSigningSession(signingSession);

                NotificationService.Notify(NotificationSeverity.Success, "Success", "Document signed successfully");
                NavigationManager.NavigateTo($"/document-header/{attachment.DocumentId}");
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
            }
        }





    }
}
