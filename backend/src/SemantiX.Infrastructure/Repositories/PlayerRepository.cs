using Microsoft.EntityFrameworkCore;
using SemantiX.Domain.Entities;
using SemantiX.Domain.Interfaces;
using SemantiX.Infrastructure.Data;

namespace SemantiX.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _db;
    public PlayerRepository(AppDbContext db) => _db = db;

    public Task<Player?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Players.Include(p => p.Stats).FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Player?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        _db.Players.Include(p => p.Stats).FirstOrDefaultAsync(p => p.Username == username, ct);

    public Task<Player?> GetByConnectionIdAsync(string connectionId, CancellationToken ct = default) =>
        _db.Players.FirstOrDefaultAsync(p => p.ConnectionId == connectionId, ct);

    public async Task<Player> CreateAsync(Player player, CancellationToken ct = default)
    {
        // PlayerStats avtomatik yaradılır
        player.Stats = new PlayerStats { PlayerId = player.Id };
        _db.Players.Add(player);
        return player;
    }

    public Task UpdateAsync(Player player, CancellationToken ct = default)
    {
        _db.Players.Update(player);
        return Task.CompletedTask;
    }

    public Task<PlayerStats?> GetStatsAsync(Guid playerId, CancellationToken ct = default) =>
        _db.PlayerStats.FirstOrDefaultAsync(s => s.PlayerId == playerId, ct);

    public async Task UpsertStatsAsync(PlayerStats stats, CancellationToken ct = default)
    {
        var existing = await _db.PlayerStats.FindAsync(new object[] { stats.Id }, ct);
        if (existing == null)
            _db.PlayerStats.Add(stats);
        else
            _db.PlayerStats.Update(stats);
    }
}
