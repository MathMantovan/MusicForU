using Microsoft.EntityFrameworkCore;
using MusicForU.Domain.Entities;

namespace MusicForU.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Band> Bands => Set<Band>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<PlaylistSong> PlaylistSongs => Set<PlaylistSong>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<PlaylistSong>().HasKey(ps => new { ps.PlaylistId, ps.SongId });

        b.Entity<User>().HasIndex(u => u.Email).IsUnique();

        b.Entity<Band>().HasIndex(x => x.Name);
        b.Entity<Song>().HasIndex(x => x.Title);

        b.Entity<Transaction>().Property(t => t.Amount).HasColumnType("decimal(18,2)");
        b.Entity<Plan>().Property(p => p.Price).HasColumnType("decimal(18,2)");

        b.Entity<Plan>().HasData(
            new Plan { Id = 1, Name = "Free",    Price = 0m,     DurationDays = 30 },
            new Plan { Id = 2, Name = "Premium", Price = 19.90m, DurationDays = 30 },
            new Plan { Id = 3, Name = "Family",  Price = 34.90m, DurationDays = 30 }
        );
    }
}
