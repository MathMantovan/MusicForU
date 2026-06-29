namespace MusicForU.Domain.Entities;

public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int? SongId { get; set; }
    public Song? Song { get; set; }
    public int? BandId { get; set; }
    public Band? Band { get; set; }
}
