using SemantiX.Application.DTOs;
using SemantiX.Application.Interfaces;
using SemantiX.Domain.Entities;
using SemantiX.Domain.Enums;
using SemantiX.Domain.Interfaces;

namespace SemantiX.Application.Services;

public class GameRoomService : IGameRoomService
{
    private readonly IUnitOfWork _uow;
    private readonly IAiClientService _aiClient;
    private readonly IAdaptiveLevelingService _leveling;
    private readonly ILieModeService _lieMode;

    public GameRoomService(
        IUnitOfWork uow,
        IAiClientService aiClient,
        IAdaptiveLevelingService leveling,
        ILieModeService lieMode)
    {
        _uow = uow;
        _aiClient = aiClient;
        _leveling = leveling;
        _lieMode = lieMode;
    }

    public async Task<GameRoomDto> CreateRoomAsync(CreateRoomDto request, CancellationToken ct = default)
    {
        var room = new GameRoom
        {
            GameMode = request.GameMode,
            IsPrivate = request.IsPrivate,
            MaxPlayers = request.MaxPlayers
        };

        var host = await _uow.Players.GetByIdAsync(request.HostPlayerId, ct)
                   ?? throw new InvalidOperationException("Oyunçu tapılmadı.");

        room.RoomPlayers.Add(new RoomPlayer
        {
            PlayerId = host.Id,
            IsHost = true,
            IsReady = false
        });

        await _uow.GameRooms.CreateAsync(room, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(room);
    }

    public async Task<GameRoomDto?> GetRoomAsync(Guid roomId, CancellationToken ct = default)
    {
        var room = await _uow.GameRooms.GetByIdAsync(roomId, ct);
        return room == null ? null : MapToDto(room);
    }

    public async Task<GameRoomDto?> GetRoomByCodeAsync(string code, CancellationToken ct = default)
    {
        var room = await _uow.GameRooms.GetByCodeAsync(code, ct);
        return room == null ? null : MapToDto(room);
    }

    public async Task<JoinRoomResultDto> JoinRoomAsync(string roomCode, Guid playerId, CancellationToken ct = default)
    {
        var room = await _uow.GameRooms.GetByCodeAsync(roomCode, ct);
        if (room == null)
            return new JoinRoomResultDto(false, null, "Otaq tapılmadı.", null);

        if (room.Status != RoomStatus.Waiting)
            return new JoinRoomResultDto(false, null, "Otaq artıq başlamışdır.", null);

        if (room.RoomPlayers.Count >= room.MaxPlayers)
            return new JoinRoomResultDto(false, null, "Otaq doludur.", null);

        if (room.RoomPlayers.Any(rp => rp.PlayerId == playerId))
            return new JoinRoomResultDto(true, roomCode, null, MapToDto(room));

        room.RoomPlayers.Add(new RoomPlayer { PlayerId = playerId, IsHost = false });
        await _uow.UpdateAsync(room, ct);
        await _uow.SaveChangesAsync(ct);

        return new JoinRoomResultDto(true, roomCode, null, MapToDto(room));
    }

    public async Task LeaveRoomAsync(string roomCode, Guid playerId, CancellationToken ct = default)
    {
        var room = await _uow.GameRooms.GetByCodeAsync(roomCode, ct);
        if (room == null) return;

        var rp = room.RoomPlayers.FirstOrDefault(x => x.PlayerId == playerId);
        if (rp != null)
            room.RoomPlayers.Remove(rp);

        if (!room.RoomPlayers.Any())
            room.Status = RoomStatus.Abandoned;

        await _uow.GameRooms.UpdateAsync(room, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<GuessResponseDto> SubmitGuessAsync(SubmitGuessDto request, CancellationToken ct = default)
    {
        var room = await _uow.GameRooms.GetByIdAsync(request.RoomId, ct)
                   ?? throw new InvalidOperationException("Otaq tapılmadı.");

        var round = room.Rounds.LastOrDefault(r => !r.IsCompleted)
                    ?? throw new InvalidOperationException("Aktiv raund yoxdur.");

        var targetWord = round.CurrentTargetWord
                         ?? throw new InvalidOperationException("Hədəf söz yoxdur.");

        var word = request.Word.Trim().ToLowerInvariant();
        var similarity = await _aiClient.GetSimilarityAsync(word, targetWord, ct);

        // Lie Mode məntiqi
        bool isLie = false;
        string? lieMessage = null;
        var resultToSend = similarity;

        if (round.IsLieMode && _lieMode.ShouldLie())
        {
            isLie = true;
            lieMessage = _lieMode.GetSarcasticMessage(similarity.Similarity);
            resultToSend = _lieMode.GenerateFakeResult(similarity);
        }

        // Guess-i saxla
        var attemptNumber = round.Guesses.Count(g => g.PlayerId == request.PlayerId) + 1;
        var guess = new Guess
        {
            RoundId = round.Id,
            PlayerId = request.PlayerId,
            Word = word,
            TargetWord = targetWord,
            Similarity = similarity.Similarity,
            Rank = similarity.Rank,
            IsLieResult = isLie,
            IsCorrect = word == targetWord.ToLowerInvariant() || similarity.Rank == 1,
            AttemptNumber = attemptNumber,
        };
        round.Guesses.Add(guess);

        // MultiTarget: tapıldıqda digər sözlər üçün hint
        string? hint = null;
        if (guess.IsCorrect && !round.DiscoveredWords.Contains(targetWord))
        {
            round.DiscoveredWords = round.DiscoveredWords.Append(targetWord).ToArray();
            var nextTarget = round.CurrentTargetWord;
            if (nextTarget != null)
                hint = $"🎯 '{targetWord}' tapıldı! Növbəti ipucu: {nextTarget[0]}***";
        }

        await _uow.GameRooms.UpdateAsync(room, ct);
        await _uow.SaveChangesAsync(ct);

        return new GuessResponseDto(
            word,
            resultToSend.Similarity,
            resultToSend.Rank,
            resultToSend.ColorCategory,
            guess.IsCorrect,
            isLie,
            lieMessage,
            hint
        );
    }

    public async Task<RoundResultDto> StartRoundAsync(Guid roomId, CancellationToken ct = default)
    {
        var room = await _uow.GameRooms.GetByIdAsync(roomId, ct)
                   ?? throw new InvalidOperationException("Otaq tapılmadı.");

        var hostPlayer = room.RoomPlayers.FirstOrDefault(rp => rp.IsHost)?.PlayerId
                         ?? room.RoomPlayers.First().PlayerId;

        var difficulty = await _leveling.GetDifficultyForPlayerAsync(hostPlayer, ct);
        var diffStr = difficulty.ToString().ToLower();

        // Oyun rejiminə görə hədəf sözlər seç
        string[] targetWords = room.GameMode switch
        {
            GameMode.MultiTarget => await _aiClient.GetRandomWordsAsync(diffStr, 3, ct),
            _ => new[] { await _aiClient.GetRandomWordAsync(diffStr, ct) }
        };

        var round = new GameRound
        {
            RoomId = roomId,
            RoundNumber = room.Rounds.Count + 1,
            TargetWords = targetWords,
            Difficulty = difficulty,
            IsLieMode = room.GameMode == GameMode.LieMode
        };

        room.Rounds.Add(round);
        room.Status = RoomStatus.InProgress;
        room.StartedAt ??= DateTime.UtcNow;

        await _uow.GameRooms.UpdateAsync(room, ct);
        await _uow.SaveChangesAsync(ct);

        return new RoundResultDto(round.Id, targetWords, difficulty, round.IsLieMode, round.StartedAt);
    }

    private static GameRoomDto MapToDto(GameRoom room) => new(
        room.Id,
        room.RoomCode,
        room.GameMode,
        room.Status,
        room.IsPrivate,
        room.RoomPlayers.Select(rp => new RoomPlayerDto(
            rp.PlayerId,
            rp.Player?.Username ?? "?",
            rp.IsHost,
            rp.IsReady,
            rp.Score
        )).ToList(),
        room.CreatedAt
    );
}
