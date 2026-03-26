using SemantiX.Domain.ValueObjects;

namespace SemantiX.Application.Interfaces;

public interface ISimilarityService
{
    /// <summary>
    /// Təxmin edilən sözün hədəf sözə semantik yaxınlığını qaytarır.
    /// </summary>
    Task<SimilarityResult> GetSimilarityAsync(string guess, string targetWord, CancellationToken ct = default);
}
