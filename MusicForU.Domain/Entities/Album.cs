namespace MusicForU.Domain.Entities;

public class Album
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int BandId { get; set; }
    public Band Band { get; set; } = null!;
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
