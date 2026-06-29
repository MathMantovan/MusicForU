using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;

namespace MusicForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlaylistController : ControllerBase
{
    private readonly IPlaylistService _service;
    public PlaylistController(IPlaylistService service) => _service = service;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlaylistDto dto)
        => Ok(await _service.CreateAsync(UserId, dto));

    [HttpGet]
    public async Task<IActionResult> GetMine()
        => Ok(await _service.GetByUserAsync(UserId));

    [HttpPost("{playlistId:int}/songs/{songId:int}")]
    public async Task<IActionResult> AddSong(int playlistId, int songId)
    {
        await _service.AddSongAsync(UserId, playlistId, songId);
        return Ok();
    }

    [HttpDelete("{playlistId:int}/songs/{songId:int}")]
    public async Task<IActionResult> RemoveSong(int playlistId, int songId)
    {
        await _service.RemoveSongAsync(UserId, playlistId, songId);
        return Ok();
    }
}
