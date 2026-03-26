namespace SemantiX.Domain.Entities;

public class Guess
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoundId { get; set; }
    public Guid PlayerId { get; set; }
    public string Word { get; set; } = string.Empty;
    public string TargetWord { get; set; } = string.Empty;
    public float Similarity { get; set; }   // 0.0 - 1.0
    public int Rank { get; set; }           // 1 = ən yaxın
    public bool IsLieResult { get; set; }  // Lie Mode-da AI yalançı nəticə verdi
    public bool IsCorrect { get; set; }    // Target tapıldı
    public int AttemptNumber { get; set; }
    public DateTime GuessedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public GameRound? Round { get; set; }
    public Player? Player { get; set; }
}
