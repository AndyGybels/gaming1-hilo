using Gaming1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gaming1.Infrastructure.Persistence;

public class GameDbContext : DbContext
{
    public DbSet<Game> Games => Set<Game>();

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Min);
            builder.Property(x => x.Max);
            builder.Property(x => x.Secret);
            builder.Property(x => x.IsOver);
            builder.Property(x => x.Winner);
            builder.Property(x => x.Attempts);
        });
    }
}