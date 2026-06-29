namespace MusicForU.Domain.Entities;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;
}
