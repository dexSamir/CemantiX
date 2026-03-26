using SemantiX.Domain.ValueObjects;

namespace SemantiX.Application.Interfaces;

/// <summary>
/// Python AI microservice ilə HTTP körpüsü.
/// </summary>
public interface IAiClientService
{
    Task<SimilarityResult> GetSimilarityAsync(string guess, string target, CancellationToken ct = default);
    Task<(float speed, float logic, float consistency)> GetThinkingScoreAsync(
        IEnumerable<(string word, float similarity, DateTime timestamp)> guesses,
        TimeSpan duration,
        CancellationToken ct = default);
    Task<string> GetRandomWordAsync(string difficulty, CancellationToken ct = default);
    Task<string[]> GetRandomWordsAsync(string difficulty, int count, CancellationToken ct = default);
}
