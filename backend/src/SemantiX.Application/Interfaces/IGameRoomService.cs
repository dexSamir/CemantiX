using SemantiX.Application.DTOs;
using SemantiX.Domain.Enums;

namespace SemantiX.Application.Interfaces;

public interface IGameRoomService
{
    Task<GameRoomDto> CreateRoomAsync(CreateRoomDto request, CancellationToken ct = default);
    Task<GameRoomDto?> GetRoomAsync(Guid roomId, CancellationToken ct = default);
    Task<GameRoomDto?> GetRoomByCodeAsync(string code, CancellationToken ct = default);
    Task<JoinRoomResultDto> JoinRoomAsync(string roomCode, Guid playerId, CancellationToken ct = default);
    Task LeaveRoomAsync(string roomCode, Guid playerId, CancellationToken ct = default);
    Task<GuessResponseDto> SubmitGuessAsync(SubmitGuessDto request, CancellationToken ct = default);
    Task<RoundResultDto> StartRoundAsync(Guid roomId, CancellationToken ct = default);
}
