using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Graph.Models;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using RhinoTicketingSystem.Components.Dialogs;

using RhinoTicketingSystem.Models;
using RhinoTicketingSystem.Models.DbA79800Ticket;
using System.Net.Mail;
using static RhinoTicketingSystem.Components.Dialogs.CreateFolderDialog;

namespace RhinoTicketingSystem.Components.Pages.Archieving
{
    public partial class ManageArchive
    {
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private IWebHostEnvironment WebHostEnvironment { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private db_a79800_ticketService db_a79800_ticketService { get; set; }

        private string rootPath;
        private string currentDirectory;
        private List<string> currentPath = new();
        private bool isUploading;

        private IEnumerable<System.IO.DirectoryInfo> directories = Enumerable.Empty<System.IO.DirectoryInfo>();
        private IEnumerable<System.IO.FileInfo> files = Enumerable.Empty<System.IO.FileInfo>();

        protected override void OnInitialized()
        {
            rootPath = Path.Combine(WebHostEnvironment.WebRootPath, "Upload", "OneDrive - Albassami");
            currentDirectory = rootPath;
            LoadCurrentDirectory(currentDirectory);
        }

        private void LoadCurrentDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = rootPath;

            var dir = new System.IO.DirectoryInfo(path);
            directories = dir.GetDirectories();
            files = dir.GetFiles();
            currentDirectory = path;
            UpdateBreadcrumbs();
        }

        private void UpdateBreadcrumbs()
        {
            currentPath.Clear();
            var relativePath = currentDirectory.Replace(rootPath, "").Trim(Path.DirectorySeparatorChar);
            if (!string.IsNullOrEmpty(relativePath))
            {
                currentPath.AddRange(relativePath.Split(Path.DirectorySeparatorChar));
            }
        }

        [Inject] private NavigationManager NavigationManager { get; set; }

        private void NavigateToFolder(string path)
        {
            LoadCurrentDirectory(path ?? rootPath);
            StateHasChanged();
        }

        private void OnDirectorySelect(TreeEventArgs args)
        {
            if (args.Value is System.IO.DirectoryInfo dir)
            {
                LoadCurrentDirectory(dir.FullName);
                StateHasChanged();
            }
        }

        private async Task CreateNewFolder()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            var result = await DialogService.OpenAsync<CreateFolderDialog>("Create New Folder",
                new Dictionary<string, object>()
                {
                    { "CurrentPath", currentDirectory }  // Parameter name must match exactly
                });

            if (result != null)
            {
                var folderModel = result as FolderModel;
                if (folderModel != null)
                {
                    var newFolderPath = Path.Combine(currentDirectory, folderModel.FolderName);
                    Directory.CreateDirectory(newFolderPath);

                    var header = new OneDriveArchivingHeader
                    {
                        FolderName = folderModel.FolderName,
                        CreatedBy = user.Identity.Name,
                        Description = folderModel.Description
                    };

                    await db_a79800_ticketService.CreateArchiveHeader(header);
                    LoadCurrentDirectory(currentDirectory);
                }
            }
        }

        private string GetPathUpToIndex(int index)
        {
            if (index < 0)
                return rootPath;

            var selectedPaths = currentPath.Take(index + 1);
            return Path.Combine(rootPath, Path.Combine(selectedPaths.ToArray()));
        }

        private List<OneDriveFileAttachment> attachmentList = new();

        private async Task HandleFileSelected(UploadChangeEventArgs args)
        {
            if (args.Files != null && args.Files.Any())
            {
                try
                {
                    isUploading = true;
                    var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                    var user = authState.User;

                    // Get current folder's header first
                    var currentFolder = Path.GetFileName(currentDirectory);
                    var header = await db_a79800_ticketService.GetArchiveHeaderByFolderName(currentFolder);

                    if (header == null)
                    {
                        NotificationService.Notify(NotificationSeverity.Error,
                            "Error",
                            "Could not find the current folder in database");
                        return;
                    }

                    foreach (var file in args.Files)
                    {
                        // Validate file size
                        if (file.Size > 10 * 1024 * 1024) // 10MB limit
                        {
                            NotificationService.Notify(NotificationSeverity.Error,
                                "Error",
                                $"File {file.Name} exceeds 10MB size limit");
                            continue;
                        }

                        var filePath = GetUniqueFileName(Path.Combine(currentDirectory, file.Name));

                        try
                        {
                            using (var fileStream = File.Create(filePath))
                            {
                                var stream = file.OpenReadStream();
                                await stream.CopyToAsync(fileStream);
                            }

                            var fileInfo = new System.IO.FileInfo(filePath);
                            var detail = new OneDriveArchivingDetail
                            {
                                ArchiveId = header.ArchiveId,  // Add this line
                                AttachedFileName = file.Name,
                                AttachedFilePath = filePath,
                                FileType = Path.GetExtension(file.Name),
                                AttachedFileSize = FormatFileSize(fileInfo.Length),
                                CreatedBy = user.Identity.Name,
                                CreatedDate = DateTime.Now
                            };

                            await db_a79800_ticketService.CreateArchiveDetail(detail);
                        }
                        catch (Exception ex)
                        {
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            throw;
                        }
                    }
                    NotificationService.Notify(NotificationSeverity.Success, "Success", "Files uploaded successfully");
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(NotificationSeverity.Error,
                        "Error",
                        $"Failed to upload files: {ex.Message}");
                }
                finally
                {
                    isUploading = false;
                    LoadCurrentDirectory(currentDirectory);
                }
            }
        }

        private string GetUniqueFileName(string filePath)
        {
            if (!File.Exists(filePath)) return filePath;

            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            int counter = 1;

            while (File.Exists(filePath))
            {
                string newFileName = $"{fileName}({counter}){extension}";
                filePath = Path.Combine(directory, newFileName);
                counter++;
            }

            return filePath;
        }

        private async Task DeleteFile(System.IO.FileInfo file)
        {
            var confirmed = await DialogService.Confirm(
                $"Are you sure you want to delete '{file.Name}'?",
                "Confirm Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed == true)
            {
                try
                {
                    file.Delete();
                    await db_a79800_ticketService.DeleteArchiveDetail(file.Name);
                    NotificationService.Notify(NotificationSeverity.Success, "Success", "File deleted successfully");
                    LoadCurrentDirectory(currentDirectory);
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Failed to delete file");
                }
            }
        }

        private async Task DownloadFile(System.IO.FileInfo file)
        {
            try
            {
                var fileBytes = await File.ReadAllBytesAsync(file.FullName);
                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", file.Name, Convert.ToBase64String(fileBytes));
                NotificationService.Notify(NotificationSeverity.Success, "Success", "File downloaded successfully");
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Failed to download file");
            }
        }

        private string GetFileIcon(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "picture_as_pdf",
                ".doc" or ".docx" => "description",
                ".xls" or ".xlsx" => "table_chart",
                ".jpg" or ".jpeg" or ".png" => "image",
                _ => "insert_drive_file"
            };
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }

        private async Task OnUploadClick()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            var currentFolder = Path.GetFileName(currentDirectory);
            var header = await db_a79800_ticketService.GetArchiveHeaderByFolderName(currentFolder);

            var result = await DialogService.OpenAsync<UploadDialog>("Upload Files", null);
            if (result is List<OneDriveFileAttachment> uploadedFiles && uploadedFiles.Any())
            {
                foreach (var attachment in uploadedFiles)
                {
                    try
                    {
                        var filePath = GetUniqueFileName(Path.Combine(currentDirectory, attachment.Name));
                        await File.WriteAllBytesAsync(filePath, attachment.FileContent);

                        var detail = new OneDriveArchivingDetail
                        {
                            ArchiveId = header.ArchiveId,
                            AttachedFileName = Path.GetFileName(filePath),
                            AttachedFilePath = filePath,
                            FileType = attachment.Type,
                            AttachedFileSize = attachment.Size,
                            CreatedBy = user.Identity.Name,
                            CreatedDate = DateTime.Now
                        };

                        await db_a79800_ticketService.CreateArchiveDetail(detail);
                    }
                    catch (Exception ex)
                    {
                        NotificationService.Notify(NotificationSeverity.Error,
                            "Error",
                            $"Failed to process file {attachment.Name}: {ex.Message}");
                    }
                }

                LoadCurrentDirectory(currentDirectory);
                StateHasChanged();
            }
        }

        private async Task EditFolderName(System.IO.DirectoryInfo directory)
        {
            var newFolderName = await JSRuntime.InvokeAsync<string>("prompt", $"Enter new folder name for {directory.Name}");
            if (!string.IsNullOrEmpty(newFolderName))
            {
                var newPath = Path.Combine(directory.Parent.FullName, newFolderName);
                Directory.Move(directory.FullName, newPath);
                // Refresh the directory list
                LoadCurrentDirectory(directory.Parent.FullName);
            }
        }

        private async Task DeleteFolder(System.IO.DirectoryInfo directory)
        {
            if (directory.GetFiles().Any() || directory.GetDirectories().Any())
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Cannot delete folder. The folder is not empty.");
                return;
            }

            var confirmed = await DialogService.Confirm(
                $"Are you sure you want to delete the folder '{directory.Name}'?",
                "Confirm Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed == true)
            {
                try
                {
                    Directory.Delete(directory.FullName, true);
                    NotificationService.Notify(NotificationSeverity.Success, "Success", "Folder deleted successfully");
                    LoadCurrentDirectory(directory.Parent.FullName);
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Failed to delete folder");
                }
            }
        }

    }
}
