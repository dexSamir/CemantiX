using SemantiX.Domain.Interfaces;
using SemantiX.Infrastructure.Data;
using SemantiX.Infrastructure.Repositories;

namespace SemantiX.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private bool _disposed;

    public IGameRoomRepository GameRooms { get; }
    public IPlayerRepository Players { get; }
    public ILeaderboardRepository Leaderboard { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        GameRooms = new GameRoomRepository(db);
        Players = new PlayerRepository(db);
        Leaderboard = new LeaderboardRepository(db);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);

    public void Dispose()
    {
        if (!_disposed)
        {
            _db.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
