using SemantiX.Domain.Enums;

namespace SemantiX.Domain.Entities;

public class PlayerStats
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public float AvgAttempts { get; set; } = 10f;
    public float WinRate { get; set; } = 0.5f;
    public float AvgTimeSeconds { get; set; } = 120f;
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public DifficultyLevel CurrentDifficulty { get; set; } = DifficultyLevel.Easy;
    public int PlayerLevel { get; set; } = 1;  // 1-10
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Player? Player { get; set; }
}
