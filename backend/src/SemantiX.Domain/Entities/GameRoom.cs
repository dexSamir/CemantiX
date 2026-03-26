using SemantiX.Domain.Enums;

namespace SemantiX.Domain.Entities;

public class GameRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RoomCode { get; set; } = GenerateCode();
    public GameMode GameMode { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Waiting;
    public int MaxPlayers { get; set; } = 2;
    public bool IsPrivate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    // Navigation
    public ICollection<GameRound> Rounds { get; set; } = new List<GameRound>();
    public ICollection<RoomPlayer> RoomPlayers { get; set; } = new List<RoomPlayer>();

    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
