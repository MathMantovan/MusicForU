using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;

namespace MusicForU.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _factory;
    public HomeController(IHttpClientFactory factory) => _factory = factory;

    private HttpClient CreateAuthClient()
    {
        var client = _factory.CreateClient("Api");
        var token = HttpContext.Session.GetString("Token");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = CreateAuthClient();

        var subResponse = await client.GetAsync("api/subscription/mine");
        if (!subResponse.IsSuccessStatusCode)
            return RedirectToAction("ChoosePlan", "Subscription");

        var favorites = await client.GetFromJsonAsync<FavoritesResultDto>("api/favorite")
            ?? new FavoritesResultDto(new(), new());

        var playlists = await client.GetFromJsonAsync<List<PlaylistDto>>("api/playlist")
            ?? new();

        var allSongs = await client.GetFromJsonAsync<List<SongResultDto>>("api/search/songs/all") ?? new();

        List<SongResultDto> searchResults = new();
        if (!string.IsNullOrWhiteSpace(q))
        {
            searchResults = await client.GetFromJsonAsync<List<SongResultDto>>(
                $"api/search/songs?q={Uri.EscapeDataString(q)}") ?? new();
        }

        ViewBag.Query = q;
        ViewBag.FavoriteSongIds = favorites.Songs.Select(s => s.SongId).ToList();
        ViewBag.FavoriteSongs = favorites.Songs;
        ViewBag.AllSongs = allSongs;
        ViewBag.Playlists = playlists;

        return View(searchResults);
    }

    [HttpPost]
    public async Task<IActionResult> FavoriteSong(int songId, string? q)
    {
        var client = CreateAuthClient();
        await client.PostAsync($"api/favorite/song/{songId}", null);
        return RedirectToAction("Index", new { q });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFavoriteSong(int songId, string? q)
    {
        var client = CreateAuthClient();
        await client.DeleteAsync($"api/favorite/song/{songId}");
        return RedirectToAction("Index", new { q });
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlaylist(string name, string? q)
    {
        var client = CreateAuthClient();
        await client.PostAsJsonAsync("api/playlist", new CreatePlaylistDto(name));
        return RedirectToAction("Index", new { q });
    }

    [HttpPost]
    public async Task<IActionResult> AddToPlaylist(int playlistId, int songId, string? q)
    {
        var client = CreateAuthClient();
        await client.PostAsync($"api/playlist/{playlistId}/songs/{songId}", null);
        return RedirectToAction("Index", new { q });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromPlaylist(int playlistId, int songId, string? q)
    {
        var client = CreateAuthClient();
        await client.DeleteAsync($"api/playlist/{playlistId}/songs/{songId}");
        return RedirectToAction("Index", new { q });
    }
}
