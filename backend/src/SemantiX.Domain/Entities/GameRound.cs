using SemantiX.Domain.Enums;

namespace SemantiX.Domain.Entities;

public class GameRound
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomId { get; set; }
    public int RoundNumber { get; set; } = 1;
    public string[] TargetWords { get; set; } = Array.Empty<string>(); // MultiTarget üçün 1-4 söz
    public string[] DiscoveredWords { get; set; } = Array.Empty<string>();
    public DifficultyLevel Difficulty { get; set; }
    public bool IsLieMode { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public Guid? WinnerPlayerId { get; set; }

    // Navigation
    public GameRoom? Room { get; set; }
    public ICollection<Guess> Guesses { get; set; } = new List<Guess>();

    // Aktiv hədəf söz (MultiTarget-da sırayla)
    public string? CurrentTargetWord => TargetWords
        .FirstOrDefault(w => !DiscoveredWords.Contains(w));

    public bool IsCompleted => TargetWords.All(w => DiscoveredWords.Contains(w))
                               || EndedAt.HasValue;
}
