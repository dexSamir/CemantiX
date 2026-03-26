using SemantiX.Application.DTOs;
using SemantiX.Application.Interfaces;
using SemantiX.Domain.Entities;
using SemantiX.Domain.ValueObjects;

namespace SemantiX.Application.Services;

public class ThinkingScoreService : IThinkingScoreService
{
    private readonly IAiClientService _aiClient;

    public ThinkingScoreService(IAiClientService aiClient)
    {
        _aiClient = aiClient;
    }

    public async Task<ThinkingScoreDto> CalculateScoreAsync(
        IEnumerable<Guess> guesses,
        TimeSpan roundDuration,
        CancellationToken ct = default)
    {
        var guessList = guesses.OrderBy(g => g.GuessedAt).ToList();
        if (!guessList.Any())
            return new ThinkingScoreDto(0, 0, 0, 0, "🎲 Şanslı");

        // Python AI servisinə göndər
        var guessData = guessList.Select(g => (g.Word, g.Similarity, g.GuessedAt));
        var (speed, logic, consistency) = await _aiClient.GetThinkingScoreAsync(guessData, roundDuration, ct);

        var ts = new ThinkingScore(speed, logic, consistency);

        return new ThinkingScoreDto(
            ts.Score,
            ts.SpeedFactor,
            ts.LogicFactor,
            ts.ConsistencyFactor,
            ts.Grade
        );
    }
}
