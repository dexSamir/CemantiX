using SemantiX.Domain.ValueObjects;

namespace SemantiX.Application.Interfaces;

/// <summary>
/// Lie Mode üçün sarkastik AI davranışı.
/// </summary>
public interface ILieModeService
{
    /// <summary>
    /// %30 ehtimalla true qaytarır — AI yalan danışacaq.
    /// </summary>
    bool ShouldLie();

    /// <summary>
    /// Yalançı nəticə generat edir.
    /// </summary>
    SimilarityResult GenerateFakeResult(SimilarityResult real);

    /// <summary>
    /// Sarkastik Azərbaycan dilindəki mesajlardan birini qaytarır.
    /// </summary>
    string GetSarcasticMessage(float realSimilarity);
}
