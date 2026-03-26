using SemantiX.Domain.Entities;
using SemantiX.Domain.Enums;

namespace SemantiX.Domain.Interfaces;

public interface ILeaderboardRepository
{
    Task<IEnumerable<LeaderboardEntry>> GetByModeAndPeriodAsync(
        GameMode mode, string period, int limit = 50, CancellationToken ct = default);
    Task<LeaderboardEntry?> GetPlayerBestAsync(
        Guid playerId, GameMode mode, CancellationToken ct = default);
    Task AddOrUpdateEntryAsync(LeaderboardEntry entry, CancellationToken ct = default);
}
