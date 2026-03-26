using SemantiX.Application.DTOs;
using SemantiX.Domain.Entities;

namespace SemantiX.Application.Interfaces;

public interface IThinkingScoreService
{
    /// <summary>
    /// Oyunçunun raund boyunca etdiyi təxminləri analiz edərək "Düşünmə Balı" hesablayır.
    /// Sürət, məntiq və ardıcıllığı nəzərə alır.
    /// </summary>
    Task<ThinkingScoreDto> CalculateScoreAsync(
        IEnumerable<Guess> guesses,
        TimeSpan roundDuration,
        CancellationToken ct = default);
}
