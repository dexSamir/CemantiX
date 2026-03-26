using SemantiX.Application.DTOs;
using SemantiX.Domain.Enums;

namespace SemantiX.Application.Interfaces;

public interface ILeaderboardService
{
    Task<IEnumerable<LeaderboardEntryDto>> GetDailyAsync(GameMode mode, CancellationToken ct = default);
    Task<IEnumerable<LeaderboardEntryDto>> GetWeeklyAsync(GameMode mode, CancellationToken ct = default);
    Task<IEnumerable<LeaderboardEntryDto>> GetAllTimeAsync(GameMode mode, CancellationToken ct = default);
    Task<IEnumerable<LeaderboardEntryDto>> GetSpeedrunTopAsync(CancellationToken ct = default);
    Task SubmitScoreAsync(SubmitScoreDto dto, CancellationToken ct = default);
}
