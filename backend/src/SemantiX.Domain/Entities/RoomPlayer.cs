namespace SemantiX.Domain.Entities;

/// <summary>
/// Bir oyunçunun bir otaqdakı varlığını göstərir.
/// </summary>
public class RoomPlayer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomId { get; set; }
    public Guid PlayerId { get; set; }
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public int Score { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public GameRoom? Room { get; set; }
    public Player? Player { get; set; }
}
