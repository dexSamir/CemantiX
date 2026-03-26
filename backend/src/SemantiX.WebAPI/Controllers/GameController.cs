using Microsoft.AspNetCore.Mvc;
using SemantiX.Application.DTOs;
using SemantiX.Application.Interfaces;
using SemantiX.Domain.Enums;

namespace SemantiX.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameRoomService _roomService;
    private readonly IThinkingScoreService _thinkingScore;
    private readonly IAdaptiveLevelingService _leveling;

    public GameController(
        IGameRoomService roomService,
        IThinkingScoreService thinkingScore,
        IAdaptiveLevelingService leveling)
    {
        _roomService = roomService;
        _thinkingScore = thinkingScore;
        _leveling = leveling;
    }

    /// <summary>
    /// Solo rejim üçün yeni otaq yaradır.
    /// </summary>
    [HttpPost("create-room")]
    public async Task<ActionResult<GameRoomDto>> CreateRoom([FromBody] CreateRoomDto request)
    {
        var room = await _roomService.CreateRoomAsync(request);
        return Ok(room);
    }

    /// <summary>
    /// Otaq detallarını qaytarır.
    /// </summary>
    [HttpGet("room/{roomId:guid}")]
    public async Task<ActionResult<GameRoomDto>> GetRoom(Guid roomId)
    {
        var room = await _roomService.GetRoomAsync(roomId);
        return room == null ? NotFound("Otaq tapılmadı.") : Ok(room);
    }

    /// <summary>
    /// Otaq koduna görə detalları qaytarır.
    /// </summary>
    [HttpGet("room/code/{code}")]
    public async Task<ActionResult<GameRoomDto>> GetRoomByCode(string code)
    {
        var room = await _roomService.GetRoomByCodeAsync(code);
        return room == null ? NotFound("Otaq tapılmadı.") : Ok(room);
    }

    /// <summary>
    /// Solo rejim üçün təxmin göndərmə (SignalR olmadan).
    /// </summary>
    [HttpPost("guess")]
    public async Task<ActionResult<GuessResponseDto>> SubmitGuess([FromBody] SubmitGuessDto request)
    {
        try
        {
            var result = await _roomService.SubmitGuessAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Yeni raund başlatmaq.
    /// </summary>
    [HttpPost("room/{roomId:guid}/start")]
    public async Task<ActionResult<RoundResultDto>> StartRound(Guid roomId)
    {
        try
        {
            var round = await _roomService.StartRoundAsync(roomId);
            return Ok(round);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Oyunçu üçün çətinlik səviyyəsini qaytarır.
    /// </summary>
    [HttpGet("difficulty/{playerId:guid}")]
    public async Task<ActionResult<DifficultyLevel>> GetDifficulty(Guid playerId)
    {
        var diff = await _leveling.GetDifficultyForPlayerAsync(playerId);
        return Ok(diff);
    }
}
