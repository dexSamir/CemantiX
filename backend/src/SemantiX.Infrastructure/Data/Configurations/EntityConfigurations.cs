using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SemantiX.Domain.Entities;

namespace SemantiX.Infrastructure.Data.Configurations;

public class GameRoomConfiguration : IEntityTypeConfiguration<GameRoom>
{
    public void Configure(EntityTypeBuilder<GameRoom> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RoomCode).HasMaxLength(6).IsRequired();
        builder.HasIndex(r => r.RoomCode).IsUnique();
        builder.Property(r => r.GameMode).IsRequired();
        builder.Property(r => r.Status).IsRequired();

        builder.HasMany(r => r.Rounds)
               .WithOne(rd => rd.Room)
               .HasForeignKey(rd => rd.RoomId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RoomPlayers)
               .WithOne(rp => rp.Room)
               .HasForeignKey(rp => rp.RoomId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class GameRoundConfiguration : IEntityTypeConfiguration<GameRound>
{
    public void Configure(EntityTypeBuilder<GameRound> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.TargetWords).HasColumnType("text[]");
        builder.Property(r => r.DiscoveredWords).HasColumnType("text[]");

        builder.HasMany(r => r.Guesses)
               .WithOne(g => g.Round)
               .HasForeignKey(g => g.RoundId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Username).HasMaxLength(30).IsRequired();
        builder.HasIndex(p => p.Username).IsUnique();

        builder.HasOne(p => p.Stats)
               .WithOne(s => s.Player)
               .HasForeignKey<PlayerStats>(s => s.PlayerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LeaderboardConfiguration : IEntityTypeConfiguration<LeaderboardEntry>
{
    public void Configure(EntityTypeBuilder<LeaderboardEntry> builder)
    {
        builder.HasKey(l => l.Id);
        builder.HasIndex(l => new { l.PlayerId, l.GameMode, l.Period });
        builder.Property(l => l.Period).HasMaxLength(10);
    }
}
