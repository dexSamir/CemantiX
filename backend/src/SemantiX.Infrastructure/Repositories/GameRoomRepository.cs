using Microsoft.EntityFrameworkCore;
using SemantiX.Domain.Entities;
using SemantiX.Domain.Interfaces;
using SemantiX.Infrastructure.Data;

namespace SemantiX.Infrastructure.Repositories;

public class GameRoomRepository : IGameRoomRepository
{
    private readonly AppDbContext _db;
    public GameRoomRepository(AppDbContext db) => _db = db;

    public Task<GameRoom?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.GameRooms
           .Include(r => r.RoomPlayers).ThenInclude(rp => rp.Player)
           .Include(r => r.Rounds).ThenInclude(rd => rd.Guesses)
           .FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<GameRoom?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        _db.GameRooms
           .Include(r => r.RoomPlayers).ThenInclude(rp => rp.Player)
           .Include(r => r.Rounds).ThenInclude(rd => rd.Guesses)
           .FirstOrDefaultAsync(r => r.RoomCode == code, ct);

    public async Task<IEnumerable<GameRoom>> GetActiveRoomsAsync(CancellationToken ct = default) =>
        await _db.GameRooms
                 .Include(r => r.RoomPlayers)
                 .Where(r => r.Status == Domain.Enums.RoomStatus.Waiting)
                 .ToListAsync(ct);

    public async Task<GameRoom> CreateAsync(GameRoom room, CancellationToken ct = default)
    {
        _db.GameRooms.Add(room);
        return room;
    }

    public Task UpdateAsync(GameRoom room, CancellationToken ct = default)
    {
        _db.GameRooms.Update(room);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var room = await GetByIdAsync(id, ct);
        if (room != null) _db.GameRooms.Remove(room);
    }
}
