using MusicForU.Application.DTOs;

namespace MusicForU.Application.Interfaces;

public interface IFavoriteService
{
    Task FavoriteSongAsync(int userId, int songId);
    Task FavoriteBandAsync(int userId, int bandId);
    Task RemoveFavoriteSongAsync(int userId, int songId);
    Task<FavoritesResultDto> GetFavoritesAsync(int userId);
}
