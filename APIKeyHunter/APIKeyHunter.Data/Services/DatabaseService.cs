using System.Text;
using System.Text.Json;
using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;
using APIKeyHunter.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace APIKeyHunter.Data.Services;

public sealed class DatabaseService : IDatabaseService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public DatabaseService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<ApiKeyModel>> GetKeysAsync(
        string? providerFilter = null,
        KeyStatus? statusFilter = null,
        CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var query = db.ApiKeys.AsQueryable();
        if (!string.IsNullOrWhiteSpace(providerFilter))
            query = query.Where(k => k.Provider == providerFilter);
        if (statusFilter.HasValue)
            query = query.Where(k => k.Status == statusFilter.Value);
        return await query.OrderByDescending(k => k.DiscoveredAt).ToListAsync(ct);
    }

    public async Task SaveKeyAsync(ApiKeyModel key, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        if (key.Id == 0)
            db.ApiKeys.Add(key);
        else
            db.ApiKeys.Update(key);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteKeyAsync(int id, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var key = await db.ApiKeys.FindAsync([id], ct);
        if (key is not null)
        {
            db.ApiKeys.Remove(key);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task UpdateStatusAsync(int id, KeyStatus status, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var key = await db.ApiKeys.FindAsync([id], ct);
        if (key is not null)
        {
            key.Status = status;
            key.LastCheckedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

    public Task<string> ExportCsvAsync(IEnumerable<ApiKeyModel> keys, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Provider,KeyPreview,Status,DiscoveredAt,LastCheckedAt");
        foreach (var key in keys)
        {
            sb.AppendLine(
                $"\"{key.Provider}\",\"{key.KeyPreview}\",\"{key.Status}\"," +
                $"\"{key.DiscoveredAt:u}\",\"{key.LastCheckedAt?.ToString("u") ?? ""}\"");
        }
        return Task.FromResult(sb.ToString());
    }

    public Task<string> ExportJsonAsync(IEnumerable<ApiKeyModel> keys, CancellationToken ct = default)
    {
        var payload = keys.Select(k => new
        {
            k.Provider,
            k.KeyPreview,
            Status = k.Status.ToString(),
            DiscoveredAt = k.DiscoveredAt.ToString("u"),
            LastCheckedAt = k.LastCheckedAt?.ToString("u")
        });
        return Task.FromResult(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }
}
