using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;

namespace MusicForU.Application.Services;

public class SearchService : ISearchService
{
    private readonly ISongRepository _songs;
    public SearchService(ISongRepository songs) => _songs = songs;

    public async Task<IEnumerable<SongResultDto>> SearchSongsAsync(string term, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(term)) return Enumerable.Empty<SongResultDto>();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var songs = await _songs.SearchAsync(term, (page - 1) * pageSize, pageSize);
        return songs.Select(s => new SongResultDto(
            s.Id, s.Title, s.Album.Title, s.Album.Band.Name));
    }

    public async Task<IEnumerable<SongResultDto>> GetAllSongsAsync()
    {
        var songs = await _songs.SearchAsync("%", 0, 500);
        return songs.Select(s => new SongResultDto(
            s.Id, s.Title, s.Album.Title, s.Album.Band.Name));
    }
}
