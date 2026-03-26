using SemantiX.Application.Interfaces;
using SemantiX.Domain.Enums;
using SemantiX.Domain.Interfaces;

namespace SemantiX.Application.Services;

public class AdaptiveLevelingService : IAdaptiveLevelingService
{
    private readonly IUnitOfWork _uow;

    public AdaptiveLevelingService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<DifficultyLevel> GetDifficultyForPlayerAsync(Guid playerId, CancellationToken ct = default)
    {
        var stats = await _uow.Players.GetStatsAsync(playerId, ct);
        if (stats == null) return DifficultyLevel.Easy;

        return stats.PlayerLevel switch
        {
            <= 3 => DifficultyLevel.Easy,
            <= 7 => DifficultyLevel.Medium,
            _ => DifficultyLevel.Hard
        };
    }

    public async Task AdjustDifficultyAsync(
        Guid playerId, int attemptsUsed, bool won, TimeSpan duration, CancellationToken ct = default)
    {
        var stats = await _uow.Players.GetStatsAsync(playerId, ct);
        if (stats == null) return;

        // Exponential Moving Average ilə ortalama cəhd
        stats.AvgAttempts = stats.AvgAttempts * 0.8f + attemptsUsed * 0.2f;
        stats.AvgTimeSeconds = stats.AvgTimeSeconds * 0.8f + (float)duration.TotalSeconds * 0.2f;

        // Win rate yenilənməsi
        var totalGames = (await _uow.Players.GetByIdAsync(playerId, ct))?.TotalGames ?? 1;
        if (totalGames > 0)
            stats.WinRate = stats.WinRate * 0.8f + (won ? 1f : 0f) * 0.2f;

        // Streak
        if (won)
        {
            stats.CurrentStreak++;
            stats.BestStreak = Math.Max(stats.BestStreak, stats.CurrentStreak);
        }
        else stats.CurrentStreak = 0;

        // Level hesablama
        stats.PlayerLevel = CalculateLevel(stats.AvgAttempts, stats.WinRate, stats.AvgTimeSeconds);
        stats.CurrentDifficulty = stats.PlayerLevel switch
        {
            <= 3 => DifficultyLevel.Easy,
            <= 7 => DifficultyLevel.Medium,
            _ => DifficultyLevel.Hard
        };
        stats.UpdatedAt = DateTime.UtcNow;

        await _uow.Players.UpsertStatsAsync(stats, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public float GetHintAccuracy(int playerLevel) => playerLevel switch
    {
        <= 3 => 0.95f,   // Yeni oyunçu: çox dəqiq hint
        <= 7 => 0.80f,   // Orta: az dəqiq
        _ => 0.60f        // Pro: az hint
    };

    private static int CalculateLevel(float avgAttempts, float winRate, float avgTime)
    {
        // Performans skoru: az cəhd + yüksək win rate + sürətli = yüksək level
        var attemptScore = Math.Max(0, (20 - avgAttempts) / 20f);  // 0-1 arası
        var winScore = winRate;                                       // 0-1 arası
        var timeScore = Math.Max(0, (300 - avgTime) / 300f);         // 0-1 arası

        var composite = attemptScore * 0.4f + winScore * 0.4f + timeScore * 0.2f;
        return (int)Math.Clamp(composite * 10, 1, 10);
    }
}
