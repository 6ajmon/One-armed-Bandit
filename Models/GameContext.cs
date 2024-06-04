using Microsoft.EntityFrameworkCore;

public class GameContext : DbContext
{
    public DbSet<PlayerContext> Players { get; set; }

    public GameContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerContext>()
            .HasIndex(b => b.Name)
            .IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=game.db");
    }
}