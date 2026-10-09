using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(g => g.Price)
                  .HasPrecision(18, 2);

            entity.Property(g => g.Name)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.HasIndex(g => g.Name);

            entity.Property(g => g.Version)
                  .IsRowVersion();
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(g => g.Name)
                  .HasMaxLength(50)
                  .IsRequired();
        });
    }
}