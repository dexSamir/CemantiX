using SemantiX.Domain.Entities;

namespace SemantiX.Domain.Interfaces;

public interface IGameRoomRepository
{
    Task<GameRoom?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<GameRoom?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IEnumerable<GameRoom>> GetActiveRoomsAsync(CancellationToken ct = default);
    Task<GameRoom> CreateAsync(GameRoom room, CancellationToken ct = default);
    Task UpdateAsync(GameRoom room, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
