using System.IO;
using System.Text;
using System.Text.Json;
using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.WPF.Services;

public sealed class ExportService : IExportService
{
    public async Task ExportToCsvAsync(IEnumerable<ApiKeyModel> keys, string filePath, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Provider,KeyPreview,Status,DiscoveredAt,LastCheckedAt");
        foreach (var k in keys)
        {
            sb.AppendLine(
                $"\"{k.Provider}\",\"{k.KeyPreview}\",\"{k.Status}\"," +
                $"\"{k.DiscoveredAt:u}\",\"{k.LastCheckedAt?.ToString("u") ?? ""}\"");
        }
        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8, ct);
    }

    public async Task ExportToJsonAsync(IEnumerable<ApiKeyModel> keys, string filePath, CancellationToken ct = default)
    {
        var payload = keys.Select(k => new
        {
            k.Provider,
            k.KeyPreview,
            Status = k.Status.ToString(),
            DiscoveredAt = k.DiscoveredAt.ToString("u"),
            LastCheckedAt = k.LastCheckedAt?.ToString("u")
        });
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8, ct);
    }
}
