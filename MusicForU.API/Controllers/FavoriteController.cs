using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.Interfaces;

namespace MusicForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _service;
    public FavoriteController(IFavoriteService service) => _service = service;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("song/{songId:int}")]
    public async Task<IActionResult> FavoriteSong(int songId)
    {
        await _service.FavoriteSongAsync(UserId, songId);
        return Ok();
    }

    [HttpDelete("song/{songId:int}")]
    public async Task<IActionResult> RemoveFavoriteSong(int songId)
    {
        await _service.RemoveFavoriteSongAsync(UserId, songId);
        return Ok();
    }

    [HttpPost("band/{bandId:int}")]
    public async Task<IActionResult> FavoriteBand(int bandId)
    {
        await _service.FavoriteBandAsync(UserId, bandId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMine() => Ok(await _service.GetFavoritesAsync(UserId));
}
