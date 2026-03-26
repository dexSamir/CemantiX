namespace SemantiX.Domain.Entities;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string? ConnectionId { get; set; }   // SignalR connection
    public int EloRating { get; set; } = 1000;
    public int TotalGames { get; set; }
    public int WonGames { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public PlayerStats? Stats { get; set; }
    public ICollection<LeaderboardEntry> LeaderboardEntries { get; set; } = new List<LeaderboardEntry>();
}
