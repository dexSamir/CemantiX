namespace SemantiX.Domain.ValueObjects;

public sealed class SimilarityResult
{
    public string Word { get; }
    public float Similarity { get; }  // 0.0 - 1.0
    public int Rank { get; }          // 1 = ən yaxın

    public SimilarityResult(string word, float similarity, int rank)
    {
        Word = word;
        Similarity = similarity;
        Rank = rank;
    }

    // Rəng şkalası üçün normalized dəyər (0-1)
    public float NormalizedScore => Math.Clamp(Similarity, 0f, 1f);

    // Rütbəyə görə rəng kateqoriyası
    public string ColorCategory => Rank switch
    {
        <= 100 => "hot",       // Parlaq yaşıl - çox yaxın
        <= 500 => "warm",      // Sarı - yaxın
        <= 1000 => "cool",     // Narıncı - orta
        _ => "cold"            // Qırmızı - uzaq
    };
}
