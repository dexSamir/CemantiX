using SemantiX.Domain.Enums;

namespace SemantiX.Application.DTOs;

public record CreateRoomDto(
    GameMode GameMode,
    Guid HostPlayerId,
    bool IsPrivate = false,
    int MaxPlayers = 2
);

public record JoinRoomResultDto(
    bool Success,
    string? RoomCode,
    string? ErrorMessage,
    GameRoomDto? Room
);

public record GameRoomDto(
    Guid Id,
    string RoomCode,
    GameMode GameMode,
    RoomStatus Status,
    bool IsPrivate,
    List<RoomPlayerDto> Players,
    DateTime CreatedAt
);

public record RoomPlayerDto(
    Guid PlayerId,
    string Username,
    bool IsHost,
    bool IsReady,
    int Score
);

public record RoundResultDto(
    Guid RoundId,
    string[] TargetWords,
    DifficultyLevel Difficulty,
    bool IsLieMode,
    DateTime StartedAt
);

public record SubmitGuessDto(
    Guid RoomId,
    Guid PlayerId,
    string Word
);

public record GuessResponseDto(
    string Word,
    float Similarity,
    int Rank,
    string ColorCategory,  // hot/warm/cool/cold
    bool IsCorrect,
    bool IsLieMode,
    string? LieMessage,    // Lie Mode sarkastik mesajı
    string? Hint           // Multi-Target: digər sözlər üçün ipucu
);

public record ThinkingScoreDto(
    float Score,
    float SpeedFactor,
    float LogicFactor,
    float ConsistencyFactor,
    string Grade
);

public record LeaderboardEntryDto(
    int Rank,
    Guid PlayerId,
    string Username,
    int Score,
    int AttemptCount,
    TimeSpan Duration,
    DateTime RecordedAt
);

public record SubmitScoreDto(
    Guid PlayerId,
    string Username,
    int Score,
    int AttemptCount,
    TimeSpan Duration,
    Domain.Enums.GameMode GameMode
);
