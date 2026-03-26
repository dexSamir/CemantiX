using SemantiX.Domain.Enums;

namespace SemantiX.Application.Interfaces;

public interface IAdaptiveLevelingService
{
    /// <summary>
    /// Oyunçunun cari səviyyəsinə uyğun çətinlik dərəcəsini qaytarır.
    /// </summary>
    Task<DifficultyLevel> GetDifficultyForPlayerAsync(Guid playerId, CancellationToken ct = default);

    /// <summary>
    /// Raund nəticəsinə əsasən oyunçunun çətinlik səviyyəsini yeniləyir.
    /// </summary>
    Task AdjustDifficultyAsync(Guid playerId, int attemptsUsed, bool won, TimeSpan duration, CancellationToken ct = default);

    /// <summary>
    /// Hint (ipucu) dəqiqliyini oyunçunun səviyyəsinə görə hesablayır.
    /// </summary>
    float GetHintAccuracy(int playerLevel);
}
