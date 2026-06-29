using Microsoft.EntityFrameworkCore;
using MusicForU.Application.Interfaces;
using MusicForU.Domain.Entities;
using MusicForU.Infrastructure.Data;

namespace MusicForU.Infrastructure.Repositories;

public class PlaylistRepository : Repository<Playlist>, IPlaylistRepository
{
    public PlaylistRepository(AppDbContext ctx) : base(ctx) { }

    public async Task<IEnumerable<Playlist>> GetByUserWithSongsAsync(int userId)
        => await _set.AsNoTracking()
            .Where(p => p.UserId == userId)
            .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song)
                .ThenInclude(s => s.Album).ThenInclude(a => a.Band)
            .ToListAsync();

    public async Task<Playlist?> GetWithSongsAsync(int playlistId)
        => await _set
            .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song)
                .ThenInclude(s => s.Album).ThenInclude(a => a.Band)
            .FirstOrDefaultAsync(p => p.Id == playlistId);
}
