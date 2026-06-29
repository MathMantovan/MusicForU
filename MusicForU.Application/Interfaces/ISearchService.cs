using MusicForU.Application.DTOs;

namespace MusicForU.Application.Interfaces;

public interface ISearchService
{
    Task<IEnumerable<SongResultDto>> SearchSongsAsync(string term, int page, int pageSize);
    Task<IEnumerable<SongResultDto>> GetAllSongsAsync();
}
