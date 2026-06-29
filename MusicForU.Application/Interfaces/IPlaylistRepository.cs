using MusicForU.Domain.Entities;

namespace MusicForU.Application.Interfaces;

public interface IPlaylistRepository : IRepository<Playlist>
{
    Task<IEnumerable<Playlist>> GetByUserWithSongsAsync(int userId);
    Task<Playlist?> GetWithSongsAsync(int playlistId);
}
