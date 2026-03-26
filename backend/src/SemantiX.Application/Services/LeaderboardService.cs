using SemantiX.Application.DTOs;
using SemantiX.Application.Interfaces;
using SemantiX.Domain.Entities;
using SemantiX.Domain.Enums;
using SemantiX.Domain.Interfaces;

namespace SemantiX.Application.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IUnitOfWork _uow;

    public LeaderboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LeaderboardEntryDto>> GetDailyAsync(GameMode mode, CancellationToken ct = default)
        => await GetEntriesAsync(mode, "daily", ct);

    public async Task<IEnumerable<LeaderboardEntryDto>> GetWeeklyAsync(GameMode mode, CancellationToken ct = default)
        => await GetEntriesAsync(mode, "weekly", ct);

    public async Task<IEnumerable<LeaderboardEntryDto>> GetAllTimeAsync(GameMode mode, CancellationToken ct = default)
        => await GetEntriesAsync(mode, "alltime", ct);

    public async Task<IEnumerable<LeaderboardEntryDto>> GetSpeedrunTopAsync(CancellationToken ct = default)
        => await GetEntriesAsync(GameMode.Speedrun, "alltime", ct);

    public async Task SubmitScoreAsync(SubmitScoreDto dto, CancellationToken ct = default)
    {
        var entry = new LeaderboardEntry
        {
            PlayerId = dto.PlayerId,
            Username = dto.Username,
            Score = dto.Score,
            AttemptCount = dto.AttemptCount,
            Duration = dto.Duration,
            GameMode = dto.GameMode,
            Period = "daily"
        };

        await _uow.Leaderboard.AddOrUpdateEntryAsync(entry, ct);

        // Həftəlik üçün ayrıca
        var weeklyEntry = new LeaderboardEntry
        {
            Id = Guid.NewGuid(),
            PlayerId = dto.PlayerId,
            Username = dto.Username,
            Score = dto.Score,
            AttemptCount = dto.AttemptCount,
            Duration = dto.Duration,
            GameMode = dto.GameMode,
            Period = "weekly"
        };
        await _uow.Leaderboard.AddOrUpdateEntryAsync(weeklyEntry, ct);

        // Alltime
        var alltimeEntry = new LeaderboardEntry
        {
            Id = Guid.NewGuid(),
            PlayerId = dto.PlayerId,
            Username = dto.Username,
            Score = dto.Score,
            AttemptCount = dto.AttemptCount,
            Duration = dto.Duration,
            GameMode = dto.GameMode,
            Period = "alltime"
        };
        await _uow.Leaderboard.AddOrUpdateEntryAsync(alltimeEntry, ct);

        await _uow.SaveChangesAsync(ct);
    }

    private async Task<IEnumerable<LeaderboardEntryDto>> GetEntriesAsync(
        GameMode mode, string period, CancellationToken ct)
    {
        var entries = await _uow.Leaderboard.GetByModeAndPeriodAsync(mode, period, 50, ct);
        return entries.Select((e, i) => new LeaderboardEntryDto(
            i + 1, e.PlayerId, e.Username, e.Score, e.AttemptCount, e.Duration, e.RecordedAt
        ));
    }
}
