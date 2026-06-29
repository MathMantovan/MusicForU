using MusicForU.Domain.Entities;

namespace MusicForU.Application.Interfaces;

public interface ISongRepository : IRepository<Song>
{
    Task<IEnumerable<Song>> SearchAsync(string term, int skip, int take);
    Task<IEnumerable<Song>> GetByIdsAsync(IEnumerable<int> ids);
}
