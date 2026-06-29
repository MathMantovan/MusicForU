namespace MusicForU.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Subscription? Subscription { get; set; }
    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
