using MusicForU.Application.DTOs;

namespace MusicForU.Application.Interfaces;

public interface IPlaylistService
{
    Task<PlaylistDto> CreateAsync(int userId, CreatePlaylistDto dto);
    Task<IEnumerable<PlaylistDto>> GetByUserAsync(int userId);
    Task AddSongAsync(int userId, int playlistId, int songId);
    Task RemoveSongAsync(int userId, int playlistId, int songId);
}
