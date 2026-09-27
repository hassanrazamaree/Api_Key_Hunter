using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Core.Interfaces;

/// <summary>Handles exporting results to various formats.</summary>
public interface IExportService
{
    Task ExportToCsvAsync(IEnumerable<ApiKeyModel> keys, string filePath, CancellationToken ct = default);
    Task ExportToJsonAsync(IEnumerable<ApiKeyModel> keys, string filePath, CancellationToken ct = default);
}
