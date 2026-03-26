using SemantiX.Domain.Entities;

namespace SemantiX.Domain.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Player?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<Player?> GetByConnectionIdAsync(string connectionId, CancellationToken ct = default);
    Task<Player> CreateAsync(Player player, CancellationToken ct = default);
    Task UpdateAsync(Player player, CancellationToken ct = default);
    Task<PlayerStats?> GetStatsAsync(Guid playerId, CancellationToken ct = default);
    Task UpsertStatsAsync(PlayerStats stats, CancellationToken ct = default);
}
