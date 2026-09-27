using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Core.Interfaces;

/// <summary>Manages authentication tokens used during scanning.</summary>
public interface ITokenService
{
    Task<IReadOnlyList<TokenModel>> GetTokensAsync(CancellationToken ct = default);
    Task<TokenModel> SaveTokenAsync(string name, string rawToken, string provider, CancellationToken ct = default);
    Task DeleteTokenAsync(int id, CancellationToken ct = default);
    Task<TokenTestStatus> TestTokenAsync(int id, CancellationToken ct = default);
}
