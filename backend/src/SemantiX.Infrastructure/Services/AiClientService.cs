using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SemantiX.Application.Interfaces;
using SemantiX.Domain.ValueObjects;

namespace SemantiX.Infrastructure.Services;

/// <summary>
/// Python AI microservice ilə HTTP körpüsü.
/// FastAPI-yə sorğular göndərir və cavabları parse edir.
/// </summary>
public class AiClientService : IAiClientService
{
    private readonly HttpClient _http;
    private readonly ILogger<AiClientService> _logger;
    private readonly string _baseUrl;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public AiClientService(HttpClient http, IConfiguration config, ILogger<AiClientService> logger)
    {
        _http = http;
        _logger = logger;
        _baseUrl = config["AiService:BaseUrl"] ?? "http://ai-service:8000";
        _http.BaseAddress = new Uri(_baseUrl);
        _http.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<SimilarityResult> GetSimilarityAsync(string guess, string target, CancellationToken ct = default)
    {
        try
        {
            var payload = new { guess, target };
            var response = await _http.PostAsJsonAsync("/similarity", payload, JsonOpts, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SimilarityResponse>(JsonOpts, ct);
            return new SimilarityResult(guess, result!.Similarity, result.Rank);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI ServisdƏn similarity alınarkən xəta: {Guess} → {Target}", guess, target);
            throw;
        }
    }

    public async Task<(float speed, float logic, float consistency)> GetThinkingScoreAsync(
        IEnumerable<(string word, float similarity, DateTime timestamp)> guesses,
        TimeSpan duration,
        CancellationToken ct = default)
    {
        try
        {
            var payload = new
            {
                guesses = guesses.Select(g => new
                {
                    word = g.word,
                    similarity = g.similarity,
                    timestamp = g.timestamp.ToString("o")
                }),
                duration_seconds = duration.TotalSeconds
            };

            var response = await _http.PostAsJsonAsync("/thinking-score", payload, JsonOpts, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ThinkingScoreResponse>(JsonOpts, ct);
            return (result!.SpeedFactor, result.LogicFactor, result.ConsistencyFactor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI Servisdən thinking score alınarkən xəta");
            return (50f, 50f, 50f); // Default bir bal qaytarılır
        }
    }

    public async Task<string> GetRandomWordAsync(string difficulty, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<RandomWordResponse>(
                $"/random-word?difficulty={difficulty}", JsonOpts, ct);
            return response!.Word;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI Servisdən random söz alınarkən xəta");
            throw;
        }
    }

    public async Task<string[]> GetRandomWordsAsync(string difficulty, int count, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<RandomWordsResponse>(
                $"/random-words?difficulty={difficulty}&count={count}", JsonOpts, ct);
            return response!.Words;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI Servisdən {Count} random söz alınarkən xəta", count);
            throw;
        }
    }

    // === Response DTOs ===
    private record SimilarityResponse(float Similarity, int Rank);
    private record ThinkingScoreResponse(float SpeedFactor, float LogicFactor, float ConsistencyFactor);
    private record RandomWordResponse(string Word);
    private record RandomWordsResponse(string[] Words);
}
