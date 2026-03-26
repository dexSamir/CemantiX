using Microsoft.EntityFrameworkCore;
using SemantiX.Domain.Entities;
using SemantiX.Domain.Enums;
using SemantiX.Domain.Interfaces;
using SemantiX.Infrastructure.Data;

namespace SemantiX.Infrastructure.Repositories;

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly AppDbContext _db;
    public LeaderboardRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<LeaderboardEntry>> GetByModeAndPeriodAsync(
        GameMode mode, string period, int limit = 50, CancellationToken ct = default)
    {
        return await _db.LeaderboardEntries
            .Where(e => e.GameMode == mode && e.Period == period)
            .OrderByDescending(e => e.Score)
            .Take(limit)
            .ToListAsync(ct);
    }

    public Task<LeaderboardEntry?> GetPlayerBestAsync(Guid playerId, GameMode mode, CancellationToken ct = default) =>
        _db.LeaderboardEntries
           .Where(e => e.PlayerId == playerId && e.GameMode == mode && e.Period == "alltime")
           .OrderByDescending(e => e.Score)
           .FirstOrDefaultAsync(ct);

    public async Task AddOrUpdateEntryAsync(LeaderboardEntry entry, CancellationToken ct = default)
    {
        var existing = await _db.LeaderboardEntries
            .FirstOrDefaultAsync(e =>
                e.PlayerId == entry.PlayerId &&
                e.GameMode == entry.GameMode &&
                e.Period == entry.Period, ct);

        if (existing == null)
            _db.LeaderboardEntries.Add(entry);
        else if (entry.Score > existing.Score)
        {
            existing.Score = entry.Score;
            existing.AttemptCount = entry.AttemptCount;
            existing.Duration = entry.Duration;
            existing.RecordedAt = DateTime.UtcNow;
        }
    }
}
