using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SemantiX.Application.Interfaces;
using SemantiX.Application.Services;
using SemantiX.Domain.Interfaces;
using SemantiX.Infrastructure.Data;
using SemantiX.Infrastructure.Repositories;
using SemantiX.Infrastructure.Services;

namespace SemantiX.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // === Database ===
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            ));

        // === Repositories ===
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IGameRoomRepository, GameRoomRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();

        // === Application Services ===
        services.AddScoped<IGameRoomService, GameRoomService>();
        services.AddScoped<IAdaptiveLevelingService, AdaptiveLevelingService>();
        services.AddScoped<IThinkingScoreService, ThinkingScoreService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<ILieModeService, LieModeService>();

        // === External Services (AI Client) ===
        services.AddHttpClient<IAiClientService, AiClientService>(client =>
        {
            var baseUrl = configuration["AiService:BaseUrl"] ?? "http://ai-service:8000";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}
