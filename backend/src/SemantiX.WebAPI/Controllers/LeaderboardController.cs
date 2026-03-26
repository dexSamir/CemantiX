using Microsoft.AspNetCore.Mvc;
using SemantiX.Application.DTOs;
using SemantiX.Application.Interfaces;
using SemantiX.Domain.Enums;

namespace SemantiX.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboard;

    public LeaderboardController(ILeaderboardService leaderboard) => _leaderboard = leaderboard;

    [HttpGet("{mode}/daily")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetDaily(GameMode mode) =>
        Ok(await _leaderboard.GetDailyAsync(mode));

    [HttpGet("{mode}/weekly")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetWeekly(GameMode mode) =>
        Ok(await _leaderboard.GetWeeklyAsync(mode));

    [HttpGet("{mode}/alltime")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetAllTime(GameMode mode) =>
        Ok(await _leaderboard.GetAllTimeAsync(mode));

    [HttpGet("speedrun")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetSpeedrunTop() =>
        Ok(await _leaderboard.GetSpeedrunTopAsync());

    [HttpPost]
    public async Task<ActionResult> Submit([FromBody] SubmitScoreDto dto)
    {
        await _leaderboard.SubmitScoreAsync(dto);
        return Ok();
    }
}
