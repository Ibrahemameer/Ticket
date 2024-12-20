using Microsoft.Graph;
using Microsoft.Graph.Models;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using RhinoTicketingSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using RhinoTicketingSystem.Data;
using Microsoft.EntityFrameworkCore;


namespace RhinoTicketingSystem.Services
{
    [Authorize]
    public class OneDriveService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationIdentityDbContext _context;

        public OneDriveService(
            GraphServiceClient graphClient,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ApplicationIdentityDbContext context)
        {
            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<DriveItem>> GetFilesAndFolders(string folderId = null)
        {
            try
            {
                if (_graphClient == null)
                    throw new InvalidOperationException("GraphServiceClient is not initialized");

                Console.WriteLine("Attempting to access drive...");
                var drive = await _graphClient.Me.Drive.GetAsync();
                Console.WriteLine($"Drive access result: {(drive != null ? "Success" : "Failed")}");

                if (string.IsNullOrEmpty(folderId))
                {
                    var items = await _graphClient.Drives[drive.Id].Items["root"].Children.GetAsync();
                    await LogFileOperation("root", "List");
                    return items.Value ?? new List<DriveItem>();
                }
                else
                {
                    var itemsList = await _graphClient.Drives[drive.Id].Items[folderId].Children.GetAsync();
                    await LogFileOperation(folderId, "List");
                    return itemsList.Value ?? new List<DriveItem>();
                }
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Service Exception: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.ResponseStatusCode}");
                Console.WriteLine($"Raw Response: {ex.RawResponseBody}");
                throw;
            }
        }
        private async Task LogFileOperation(string fileName, string operation)
        {
            try
            {
                var userId = _userManager.GetUserId(
                    _httpContextAccessor.HttpContext.User);

                var log = new FileOperationLog
                {
                    UserId = userId,
                    FileName = fileName,
                    Operation = operation,
                    Timestamp = DateTime.UtcNow
                };

                await _context.FileOperationLogs.AddAsync(log);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                    throw new Exception("Failed to save log entry");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging Error: {ex.Message}");
                throw;
            }
        }
        public async Task VerifyDatabaseConnection()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                var logsExist = await _context.FileOperationLogs.AnyAsync();
                Console.WriteLine($"Database connection successful. Logs exist: {logsExist}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                throw;
            }
        }
        public async Task<DriveItem> UploadFile(Stream fileStream, string fileName)
        {
            try
            {
                var drive = await _graphClient.Me.Drive.GetAsync();
                var result = await _graphClient.Drives[drive.Id].Root
                    .ItemWithPath(fileName)
                    .Content
                    .PutAsync(fileStream);
                return result;
            }
            catch (ServiceException ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }

        public async Task DeleteFile(string fileId)
        {
            try
            {
                var drive = await _graphClient.Me.Drive.GetAsync();
                await _graphClient.Drives[drive.Id].Items[fileId].DeleteAsync();
            }
            catch (ServiceException ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}");
            }
        }

        public async Task DownloadFile(string fileId, Stream targetStream)
        {
            try
            {
                var drive = await _graphClient.Me.Drive.GetAsync();
                var driveItem = await _graphClient.Drives[drive.Id].Items[fileId].GetAsync();

                if (driveItem.AdditionalData.TryGetValue("@microsoft.graph.downloadUrl", out object downloadUrl))
                {
                    using var httpClient = new HttpClient();
                    using var response = await httpClient.GetStreamAsync(downloadUrl.ToString());
                    await response.CopyToAsync(targetStream);
                }
                else
                {
                    throw new Exception("Download URL not found");
                }
            }
            catch (ServiceException ex)
            {
                throw new Exception($"Error downloading file: {ex.Message}");
            }
        }
    }
}