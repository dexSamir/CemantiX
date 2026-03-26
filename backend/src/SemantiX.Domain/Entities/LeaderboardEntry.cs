using SemantiX.Domain.Enums;

namespace SemantiX.Domain.Entities;

public class LeaderboardEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Score { get; set; }
    public int AttemptCount { get; set; }
    public float BestSimilarity { get; set; }
    public TimeSpan Duration { get; set; }
    public GameMode GameMode { get; set; }
    public string Period { get; set; } = "daily"; // daily, weekly, alltime
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Player? Player { get; set; }
}
