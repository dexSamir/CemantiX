namespace SemantiX.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGameRoomRepository GameRooms { get; }
    IPlayerRepository Players { get; }
    ILeaderboardRepository Leaderboard { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
