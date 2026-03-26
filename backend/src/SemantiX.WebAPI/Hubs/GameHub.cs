using Microsoft.AspNetCore.SignalR;
using SemantiX.Application.DTOs;
using SemantiX.Application.Interfaces;
using SemantiX.Domain.Entities;
using SemantiX.Domain.Enums;
using SemantiX.Domain.Interfaces;

namespace SemantiX.WebAPI.Hubs;

/// <summary>
/// Multiplayer otaq idarəetməsi üçün SignalR Hub.
/// Real-time 1v1 Battle, Private Room və canlı rəqabət.
/// </summary>
public class GameHub : Hub
{
    private readonly IGameRoomService _roomService;
    private readonly IThinkingScoreService _thinkingScore;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GameHub> _logger;

    public GameHub(
        IGameRoomService roomService,
        IThinkingScoreService thinkingScore,
        IUnitOfWork uow,
        ILogger<GameHub> logger)
    {
        _roomService = roomService;
        _thinkingScore = thinkingScore;
        _uow = uow;
        _logger = logger;
    }

    /// <summary>
    /// Oyunçu qeydiyyatı / giriş. ConnectionId saxlanılır.
    /// </summary>
    public async Task Register(string username)
    {
        var player = await _uow.Players.GetByUsernameAsync(username);
        if (player == null)
        {
            player = new Player { Username = username, ConnectionId = Context.ConnectionId };
            await _uow.Players.CreateAsync(player);
            await _uow.SaveChangesAsync();
        }
        else
        {
            player.ConnectionId = Context.ConnectionId;
            player.LastSeenAt = DateTime.UtcNow;
            await _uow.Players.UpdateAsync(player);
            await _uow.SaveChangesAsync();
        }

        await Clients.Caller.SendAsync("Registered", new
        {
            player.Id,
            player.Username,
            player.EloRating
        });
    }

    /// <summary>
    /// Yeni otaq yaratmaq.
    /// </summary>
    public async Task CreateRoom(GameMode gameMode, bool isPrivate = false)
    {
        var player = await GetCurrentPlayer();
        if (player == null) return;

        var room = await _roomService.CreateRoomAsync(new CreateRoomDto(
            gameMode, player.Id, isPrivate
        ));

        await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode);

        await Clients.Caller.SendAsync("RoomCreated", room);
        _logger.LogInformation("Otaq yaradıldı: {Code} ({Mode})", room.RoomCode, gameMode);
    }

    /// <summary>
    /// Otağa qoşulmaq.
    /// </summary>
    public async Task JoinRoom(string roomCode)
    {
        var player = await GetCurrentPlayer();
        if (player == null) return;

        var result = await _roomService.JoinRoomAsync(roomCode, player.Id);
        if (!result.Success)
        {
            await Clients.Caller.SendAsync("Error", result.ErrorMessage);
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

        // Otaqdakı hər kəsə bildiriş
        await Clients.Group(roomCode).SendAsync("PlayerJoined", new
        {
            PlayerId = player.Id,
            player.Username,
            Room = result.Room
        });

        _logger.LogInformation("{User} otağa qoşuldu: {Code}", player.Username, roomCode);
    }

    /// <summary>
    /// Otaqdan ayrılmaq.
    /// </summary>
    public async Task LeaveRoom(string roomCode)
    {
        var player = await GetCurrentPlayer();
        if (player == null) return;

        await _roomService.LeaveRoomAsync(roomCode, player.Id);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

        await Clients.Group(roomCode).SendAsync("PlayerLeft", new
        {
            PlayerId = player.Id,
            player.Username
        });
    }

    /// <summary>
    /// Raunda hazır olduğunu bildirmək.
    /// </summary>
    public async Task SetReady(string roomCode)
    {
        var player = await GetCurrentPlayer();
        if (player == null) return;

        var room = await _roomService.GetRoomByCodeAsync(roomCode);
        if (room == null) return;

        await Clients.Group(roomCode).SendAsync("PlayerReady", new
        {
            PlayerId = player.Id,
            player.Username
        });

        // Hamı hazırdırsa raund başlasın
        // Bu sadələşdirilmiş yoxlamadır - real app-da server-side state check lazımdır
        if (room.Players.Count >= 2)
        {
            await StartRound(roomCode);
        }
    }

    /// <summary>
    /// Yeni raund başlatmaq.
    /// </summary>
    public async Task StartRound(string roomCode)
    {
        var room = await _roomService.GetRoomByCodeAsync(roomCode);
        if (room == null) return;

        var round = await _roomService.StartRoundAsync(room.Id);

        // TargetWords-ı heç kimə göstərmirik, yalnız raund məlumatını göndəririk
        await Clients.Group(roomCode).SendAsync("RoundStarted", new
        {
            round.RoundId,
            round.Difficulty,
            round.IsLieMode,
            TargetCount = round.TargetWords.Length, // Neçə söz tapılmalıdır
            StartedAt = round.StartedAt
        });
    }

    /// <summary>
    /// Təxmin göndərmək. Əsas oyun məntiqi burada baş verir.
    /// </summary>
    public async Task SubmitGuess(string roomCode, string word)
    {
        var player = await GetCurrentPlayer();
        if (player == null) return;

        var room = await _roomService.GetRoomByCodeAsync(roomCode);
        if (room == null) return;

        var result = await _roomService.SubmitGuessAsync(new SubmitGuessDto(
            room.Id, player.Id, word
        ));

        // Təxmin edən oyunçuya tam nəticə
        await Clients.Caller.SendAsync("GuessResult", new
        {
            result.Word,
            result.Similarity,
            result.Rank,
            result.ColorCategory,
            result.IsCorrect,
            result.IsLieMode,
            result.LieMessage,
            result.Hint
        });

        // Rəqibə yalnız rank və rəng göstərilir (sözü görmür)
        await Clients.OthersInGroup(roomCode).SendAsync("OpponentGuessed", new
        {
            PlayerId = player.Id,
            Username = player.Username,
            result.Rank,
            result.ColorCategory,
            result.IsCorrect
        });

        // Tapıldısa raund bitir
        if (result.IsCorrect)
        {
            await HandleRoundComplete(roomCode, room.Id, player);
        }
    }

    /// <summary>
    /// Speedrun modunda vaxt bitdikdə çağırılır.
    /// </summary>
    public async Task SpeedrunTimeUp(string roomCode, int bestRank, float bestSimilarity)
    {
        var player = await GetCurrentPlayer();
        if (player == null) return;

        await Clients.Group(roomCode).SendAsync("SpeedrunResult", new
        {
            PlayerId = player.Id,
            player.Username,
            BestRank = bestRank,
            BestSimilarity = bestSimilarity
        });
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var player = await _uow.Players.GetByConnectionIdAsync(Context.ConnectionId);
        if (player != null)
        {
            player.ConnectionId = null;
            player.LastSeenAt = DateTime.UtcNow;
            await _uow.Players.UpdateAsync(player);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("{User} bağlantısı kəsildi", player.Username);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // === Private Helpers ===

    private async Task<Player?> GetCurrentPlayer()
    {
        var player = await _uow.Players.GetByConnectionIdAsync(Context.ConnectionId);
        if (player == null)
        {
            await Clients.Caller.SendAsync("Error", "Əvvəlcə Register olun.");
        }
        return player;
    }

    private async Task HandleRoundComplete(string roomCode, Guid roomId, Player winner)
    {
        var room = await _roomService.GetRoomAsync(roomId);
        if (room == null) return;

        await Clients.Group(roomCode).SendAsync("RoundComplete", new
        {
            WinnerId = winner.Id,
            WinnerUsername = winner.Username,
            Message = $"🎉 {winner.Username} sözü tapdı!"
        });
    }
}
