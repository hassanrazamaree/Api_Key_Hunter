using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Core.Interfaces;

/// <summary>Abstracts all persistence operations for API key records.</summary>
public interface IDatabaseService
{
    Task<IReadOnlyList<ApiKeyModel>> GetKeysAsync(
        string? providerFilter = null,
        KeyStatus? statusFilter = null,
        CancellationToken ct = default);

    Task SaveKeyAsync(ApiKeyModel key, CancellationToken ct = default);
    Task DeleteKeyAsync(int id, CancellationToken ct = default);
    Task UpdateStatusAsync(int id, KeyStatus status, CancellationToken ct = default);
    Task<string> ExportCsvAsync(IEnumerable<ApiKeyModel> keys, CancellationToken ct = default);
    Task<string> ExportJsonAsync(IEnumerable<ApiKeyModel> keys, CancellationToken ct = default);
}
