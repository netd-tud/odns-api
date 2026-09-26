using System.Text.Json;
using System.Text.Json.Serialization;

namespace ODNSAPI.Downloads
{
    public class DownloadFileDescriptor
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = string.Empty;
    }

    public class DownloadProtocolManifest
    {
        [JsonPropertyName("scan_date")]
        public string ScanDate { get; set; } = string.Empty;
        [JsonPropertyName("row_count")]
        public long RowCount { get; set; }
        public Dictionary<string, DownloadFileDescriptor> Files { get; set; } = new();
    }

    public class DownloadManifest
    {
        public int Version { get; set; }
        public Dictionary<string, DownloadProtocolManifest> Protocols { get; set; } = new();
    }

    public class LatestDownloadProvider
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly HashSet<string> SupportedProtocols = new(StringComparer.OrdinalIgnoreCase)
        {
            "tcp",
            "udp"
        };

        private static readonly HashSet<string> SupportedFormats = new(StringComparer.OrdinalIgnoreCase)
        {
            "csv",
            "csv.zst",
            "parquet"
        };

        private readonly string _downloadDirectory;
        private readonly string _manifestPath;
        private readonly string _internalRoute;

        public LatestDownloadProvider(IConfiguration configuration)
        {
            _downloadDirectory = configuration.GetValue<string>("Settings:Downloads:Directory")
                ?? "/data/downloads";
            _manifestPath = Path.Combine(_downloadDirectory, "manifest.json");
            _internalRoute = (configuration.GetValue<string>("Settings:Downloads:InternalRoute")
                ?? "/_downloads").TrimEnd('/');
        }

        public bool IsSupportedProtocol(string? protocol)
        {
            return protocol != null && SupportedProtocols.Contains(protocol);
        }

        public bool IsSupportedFormat(string? format)
        {
            return format != null && SupportedFormats.Contains(format);
        }

        public async Task<DownloadFileDescriptor?> GetLatest(
            string protocol,
            string format,
            CancellationToken cancellationToken)
        {
            await using FileStream manifestStream = File.OpenRead(_manifestPath);
            DownloadManifest? manifest = await JsonSerializer.DeserializeAsync<DownloadManifest>(
                manifestStream,
                JsonOptions,
                cancellationToken: cancellationToken
            );

            if (manifest?.Version != 1
                || !manifest.Protocols.TryGetValue(protocol.ToLowerInvariant(), out var protocolManifest)
                || !protocolManifest.Files.TryGetValue(format.ToLowerInvariant(), out var file))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(file.Name)
                || file.Name != Path.GetFileName(file.Name)
                || file.Size <= 0
                || string.IsNullOrWhiteSpace(file.ContentType))
            {
                return null;
            }

            FileInfo fileInfo = new(Path.Combine(_downloadDirectory, file.Name));
            if (!fileInfo.Exists || fileInfo.Length != file.Size)
                return null;

            return file;
        }

        public string GetInternalPath(DownloadFileDescriptor file)
        {
            return $"{_internalRoute}/{Uri.EscapeDataString(file.Name)}";
        }
    }
}
