using Microsoft.EntityFrameworkCore;
using MusicForU.Application.Interfaces;
using MusicForU.Domain.Entities;
using MusicForU.Infrastructure.Data;

namespace MusicForU.Infrastructure.Repositories;

public class SongRepository : Repository<Song>, ISongRepository
{
    public SongRepository(AppDbContext ctx) : base(ctx) { }

    public async Task<IEnumerable<Song>> SearchAsync(string term, int skip, int take)
        => await _set.AsNoTracking()
            .Include(s => s.Album).ThenInclude(a => a.Band)
            .Where(s => EF.Functions.Like(s.Title, $"%{term}%"))
            .OrderBy(s => s.Title)
            .Skip(skip).Take(take)
            .ToListAsync();

    public async Task<IEnumerable<Song>> GetByIdsAsync(IEnumerable<int> ids)
        => await _set.AsNoTracking()
            .Include(s => s.Album).ThenInclude(a => a.Band)
            .Where(s => ids.Contains(s.Id))
            .ToListAsync();
}
