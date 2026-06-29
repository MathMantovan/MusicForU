using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Domain.Entities;

namespace MusicForU.Application.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IPlaylistRepository _playlists;

    public PlaylistService(IPlaylistRepository playlists)
    {
        _playlists = playlists;
    }

    public async Task<PlaylistDto> CreateAsync(int userId, CreatePlaylistDto dto)
    {
        var playlist = new Playlist { Name = dto.Name, UserId = userId };
        await _playlists.AddAsync(playlist);
        await _playlists.SaveChangesAsync();
        return new PlaylistDto(playlist.Id, playlist.Name, new List<SongResultDto>());
    }

    public async Task<IEnumerable<PlaylistDto>> GetByUserAsync(int userId)
    {
        var playlists = await _playlists.GetByUserWithSongsAsync(userId);
        return playlists.Select(p => new PlaylistDto(
            p.Id,
            p.Name,
            p.PlaylistSongs.Select(ps => new SongResultDto(
                ps.Song.Id, ps.Song.Title, ps.Song.Album.Title, ps.Song.Album.Band.Name
            )).ToList()
        ));
    }

    public async Task AddSongAsync(int userId, int playlistId, int songId)
    {
        var playlist = await _playlists.GetWithSongsAsync(playlistId);
        if (playlist is null || playlist.UserId != userId) return;

        if (playlist.PlaylistSongs.Any(ps => ps.SongId == songId)) return;

        playlist.PlaylistSongs.Add(new PlaylistSong { PlaylistId = playlistId, SongId = songId });
        await _playlists.SaveChangesAsync();
    }

    public async Task RemoveSongAsync(int userId, int playlistId, int songId)
    {
        var playlist = await _playlists.GetWithSongsAsync(playlistId);
        if (playlist is null || playlist.UserId != userId) return;

        var ps = playlist.PlaylistSongs.FirstOrDefault(ps => ps.SongId == songId);
        if (ps is null) return;

        playlist.PlaylistSongs.Remove(ps);
        await _playlists.SaveChangesAsync();
    }
}
