using SemantiX.Application.Interfaces;
using SemantiX.Domain.ValueObjects;

namespace SemantiX.Application.Services;

public class LieModeService : ILieModeService
{
    private readonly Random _random = new();

    // Sarkastik Azərbaycan dilindəki mesajlar
    private static readonly string[] CloseButActuallyFarMessages =
    {
        "Vay, əla! Demək olar ki, tapdın! (Deyil 😏)",
        "Çox yaxınsan! Sadəcə... istiqamətin tam əks tərəfdə 🙃",
        "isti-isti! Jk, soyuqsan 🧊",
        "Möhtəşəm! Bu qədər yanılmaq üçün istedada ehtiyac var 🎭"
    };

    private static readonly string[] FarButActuallyCloseMessages =
    {
        "Uzaqsın! (Amma aranızda sadəcə 1 metr var 🤫)",
        "Heç vaxt tapa bilməzsən... ya da çox yaxında tapa bilərsən?",
        "Soyuq! Amma oda elə yaxınsın ki, yanırsan 🔥",
        "Bu söz? Heç ağlıma da gəlməzdi! (Amma düzdü 😌)"
    };

    public bool ShouldLie() => _random.NextDouble() < 0.30; // 30% ehtimal

    public SimilarityResult GenerateFakeResult(SimilarityResult real)
    {
        // Yaxın olarsa uzaq göstər, uzaq olarsa yaxın göstər
        if (real.Rank <= 300)
        {
            // Yaxın → uzaq göstər
            var fakeRank = _random.Next(1500, 5000);
            var fakeSim = Math.Max(0f, real.Similarity - _random.NextSingle() * 0.5f);
            return new SimilarityResult(real.Word, fakeSim, fakeRank);
        }
        else
        {
            // Uzaq → yaxın göstər
            var fakeRank = _random.Next(1, 200);
            var fakeSim = Math.Min(1f, real.Similarity + _random.NextSingle() * 0.4f);
            return new SimilarityResult(real.Word, fakeSim, fakeRank);
        }
    }

    public string GetSarcasticMessage(float realSimilarity)
    {
        if (realSimilarity > 0.6f)
        {
            // Həqiqətdə yaxın, yalandan uzaq deyirik
            return CloseButActuallyFarMessages[_random.Next(CloseButActuallyFarMessages.Length)];
        }
        else
        {
            // Həqiqətdə uzaq, yalandan yaxın deyirik
            return FarButActuallyCloseMessages[_random.Next(FarButActuallyCloseMessages.Length)];
        }
    }
}
