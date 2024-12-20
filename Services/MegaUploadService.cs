using CG.Web.MegaApiClient;

namespace RhinoTicketingSystem.Services
{
    public class MegaUploadService
    {
        private readonly IMegaApiClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MegaUploadService> _logger;

        public MegaUploadService(
            IConfiguration configuration,
            ILogger<MegaUploadService> logger)
        {
            _client = new MegaApiClient();
            _configuration = configuration;
            _logger = logger;
            InitializeClient();
        }

        private void InitializeClient()
        {
            try
            {
                var email = _configuration["Mega:Email"]
                    ?? throw new ArgumentNullException("Mega:Email configuration is missing");
                var password = _configuration["Mega:Password"]
                    ?? throw new ArgumentNullException("Mega:Password configuration is missing");

                _client.Login(email, password);
                _logger.LogInformation("Successfully initialized MEGA client");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MEGA client");
                throw;
            }
        }
        // Add these methods
        public async Task<INode> CreateFolderAsync(string name, string parentPath)
        {
            var parent = await GetParentNode(parentPath);
            return await _client.CreateFolderAsync(name, parent);
        }

        public async Task<bool> RenameNodeAsync(INode node, string newName)
        {
            await _client.RenameAsync(node, newName);
            return true;
        }

        public async Task<bool> DeleteFolderAsync(INode folder)
        {
            await _client.DeleteAsync(folder, moveToTrash: true);
            return true;
        }
        private bool ValidateFile(Stream fileStream, string fileName)
        {
            var maxFileSize = _configuration.GetValue<long>("Mega:MaxFileSize", 10 * 1024 * 1024); // 10MB default
            var allowedExtensions = _configuration.GetSection("Mega:AllowedExtensions")
                .Get<string[]>() ?? new[] { ".pdf", ".doc", ".docx", ".jpg", ".png" };

            return fileStream.Length <= maxFileSize &&
                   allowedExtensions.Contains(Path.GetExtension(fileName).ToLower());
        }
        public async Task<INode> CreateFolder(string name, string parentPath = "")
        {
            var parent = await GetParentNode(parentPath);
            return await _client.CreateFolderAsync(name, parent);
        }
        public async Task<IEnumerable<INode>> GetDirectories(string path = "")
        {
            var node = await GetParentNode(path);
            return await _client.GetNodesAsync(node);
        }

        public async Task<bool> DeleteNode(INode node)
        {
            await _client.DeleteAsync(node, false);
            return true;
        }

        public async Task<INode> RenameNode(INode node, string newName)
        {
            return await _client.RenameAsync(node, newName);
        }

        public async Task<Stream> DownloadFile(INode node)
        {
            return await _client.DownloadAsync(node);
        }
        public async Task UploadFile(Stream fileStream, string fileName, string parentPath = "")
        {
            var parent = await GetParentNode(parentPath);
            await _client.UploadAsync(fileStream, fileName, parent);
        }
        public async Task<INode> GetParentNode(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    // Get root node if no path is specified
                    var nodes = await _client.GetNodesAsync();
                    return nodes.First(x => x.Type == NodeType.Root);
                }

                // Get specific folder node by path
                var allNodes = await _client.GetNodesAsync();
                return allNodes.First(x => x.Name == path && x.Type == NodeType.Directory);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get parent node: {ex.Message}");
            }
        }
        private async Task<INode> ValidateNode(INode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var nodes = await _client.GetNodesAsync();
            return nodes.FirstOrDefault(x => x.Id == node.Id)
                ?? throw new Exception("Node not found");
        }

    }


}
