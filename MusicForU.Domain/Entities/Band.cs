namespace MusicForU.Domain.Entities;

public class Band
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public ICollection<Album> Albums { get; set; } = new List<Album>();
}
