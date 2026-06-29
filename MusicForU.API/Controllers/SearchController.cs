using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.Interfaces;

namespace MusicForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _search;
    public SearchController(ISearchService search) => _search = search;

    [HttpGet("songs")]
    public async Task<IActionResult> Songs([FromQuery] string q, int page = 1, int pageSize = 20)
        => Ok(await _search.SearchSongsAsync(q, page, pageSize));

    [HttpGet("songs/all")]
    public async Task<IActionResult> AllSongs()
        => Ok(await _search.GetAllSongsAsync());
}
