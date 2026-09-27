using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Providers.Services;

/// <summary>
/// In-memory token service stub.
/// Wire up a real implementation backed by the database when ready.
/// </summary>
public sealed class StubTokenService : ITokenService
{
    private readonly List<TokenModel> _tokens = [];
    private int _nextId = 1;

    public Task<IReadOnlyList<TokenModel>> GetTokensAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<TokenModel>>(_tokens.AsReadOnly());

    public Task<TokenModel> SaveTokenAsync(string name, string rawToken, string provider, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);

        var preview = BuildPreview(rawToken);
        var model = new TokenModel
        {
            Id = _nextId++,
            Name = name,
            Preview = preview,
            Provider = provider,
            IsActive = true,
            AddedAt = DateTime.UtcNow,
            TestStatus = TokenTestStatus.NotTested
        };
        _tokens.Add(model);
        return Task.FromResult(model);
    }

    public Task DeleteTokenAsync(int id, CancellationToken ct = default)
    {
        _tokens.RemoveAll(t => t.Id == id);
        return Task.CompletedTask;
    }

    public Task<TokenTestStatus> TestTokenAsync(int id, CancellationToken ct = default)
    {
        // Stub: always returns NotTested until a real validator is wired in.
        var token = _tokens.FirstOrDefault(t => t.Id == id);
        if (token is null) return Task.FromResult(TokenTestStatus.Invalid);
        token.LastTestedAt = DateTime.UtcNow;
        token.TestStatus = TokenTestStatus.NotTested;
        return Task.FromResult(TokenTestStatus.NotTested);
    }

    private static string BuildPreview(string raw)
    {
        if (raw.Length <= 7) return new string('•', raw.Length);
        var prefix = raw[..4];
        var suffix = raw[^3..];
        var masked = new string('•', Math.Min(raw.Length - 7, 10));
        return $"{prefix}{masked}{suffix}";
    }
}
