using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Domain.Entities;

namespace MusicForU.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IRepository<Favorite> _favorites;
    private readonly ISongRepository _songs;

    public FavoriteService(IRepository<Favorite> favorites, ISongRepository songs)
    {
        _favorites = favorites;
        _songs = songs;
    }

    public async Task FavoriteSongAsync(int userId, int songId)
    {
        var exists = (await _favorites.FindAsync(f => f.UserId == userId && f.SongId == songId)).Any();
        if (exists) return;
        await _favorites.AddAsync(new Favorite { UserId = userId, SongId = songId });
        await _favorites.SaveChangesAsync();
    }

    public async Task FavoriteBandAsync(int userId, int bandId)
    {
        var exists = (await _favorites.FindAsync(f => f.UserId == userId && f.BandId == bandId)).Any();
        if (exists) return;
        await _favorites.AddAsync(new Favorite { UserId = userId, BandId = bandId });
        await _favorites.SaveChangesAsync();
    }

    public async Task RemoveFavoriteSongAsync(int userId, int songId)
    {
        var fav = (await _favorites.FindAsync(f => f.UserId == userId && f.SongId == songId)).FirstOrDefault();
        if (fav is null) return;
        _favorites.Remove(fav);
        await _favorites.SaveChangesAsync();
    }

    public async Task<FavoritesResultDto> GetFavoritesAsync(int userId)
    {
        var all = (await _favorites.FindAsync(f => f.UserId == userId)).ToList();

        var songIds = all.Where(f => f.SongId != null).Select(f => f.SongId!.Value).ToList();
        var songs = await _songs.GetByIdsAsync(songIds);
        var songDtos = songs.Select(s => new FavoriteSongDto(s.Id, s.Title, s.Album.Title, s.Album.Band.Name)).ToList();

        var bandIds = all.Where(f => f.BandId != null).Select(f => f.BandId!.Value).ToList();
        return new FavoritesResultDto(songDtos, bandIds);
    }
}
