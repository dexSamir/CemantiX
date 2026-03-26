using Microsoft.AspNetCore.Mvc;
using SemantiX.Domain.Entities;
using SemantiX.Domain.Interfaces;

namespace SemantiX.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public PlayerController(IUnitOfWork uow) => _uow = uow;

    [HttpPost("register")]
    public async Task<ActionResult<Player>> Register([FromBody] RegisterPlayerDto dto)
    {
        var existing = await _uow.Players.GetByUsernameAsync(dto.Username);
        if (existing != null)
            return Ok(existing);

        var player = new Player { Username = dto.Username };
        await _uow.Players.CreateAsync(player);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetStats), new { playerId = player.Id }, player);
    }

    [HttpGet("{playerId:guid}/stats")]
    public async Task<ActionResult<PlayerStats>> GetStats(Guid playerId)
    {
        var stats = await _uow.Players.GetStatsAsync(playerId);
        return stats == null ? NotFound() : Ok(stats);
    }

    [HttpGet("{playerId:guid}")]
    public async Task<ActionResult<Player>> GetPlayer(Guid playerId)
    {
        var player = await _uow.Players.GetByIdAsync(playerId);
        return player == null ? NotFound() : Ok(player);
    }
}

public record RegisterPlayerDto(string Username);
