using Moq;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Application.Services;
using MusicForU.Domain.Entities;

namespace MusicForU.Tests;

public class PlaylistServiceTests
{
    private readonly Mock<IPlaylistRepository> _playlistRepo = new();
    private readonly PlaylistService _sut;

    public PlaylistServiceTests()
    {
        _sut = new PlaylistService(_playlistRepo.Object);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNewPlaylist()
    {
        var result = await _sut.CreateAsync(1, new CreatePlaylistDto("Rock Hits"));

        Assert.Equal("Rock Hits", result.Name);
        Assert.Empty(result.Songs);
        _playlistRepo.Verify(r => r.AddAsync(It.Is<Playlist>(p => p.Name == "Rock Hits" && p.UserId == 1)), Times.Once);
        _playlistRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddSongAsync_NewSong_Adds()
    {
        var playlist = new Playlist
        {
            Id = 1, UserId = 1,
            PlaylistSongs = new List<PlaylistSong>()
        };
        _playlistRepo.Setup(r => r.GetWithSongsAsync(1)).ReturnsAsync(playlist);

        await _sut.AddSongAsync(1, 1, 10);

        Assert.Single(playlist.PlaylistSongs);
        Assert.Equal(10, playlist.PlaylistSongs.First().SongId);
        _playlistRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddSongAsync_DuplicateSong_DoesNotAdd()
    {
        var playlist = new Playlist
        {
            Id = 1, UserId = 1,
            PlaylistSongs = new List<PlaylistSong> { new PlaylistSong { PlaylistId = 1, SongId = 10 } }
        };
        _playlistRepo.Setup(r => r.GetWithSongsAsync(1)).ReturnsAsync(playlist);

        await _sut.AddSongAsync(1, 1, 10);

        Assert.Single(playlist.PlaylistSongs);
        _playlistRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AddSongAsync_WrongUser_DoesNothing()
    {
        var playlist = new Playlist { Id = 1, UserId = 2, PlaylistSongs = new List<PlaylistSong>() };
        _playlistRepo.Setup(r => r.GetWithSongsAsync(1)).ReturnsAsync(playlist);

        await _sut.AddSongAsync(1, 1, 10);

        Assert.Empty(playlist.PlaylistSongs);
        _playlistRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RemoveSongAsync_Exists_Removes()
    {
        var ps = new PlaylistSong { PlaylistId = 1, SongId = 10 };
        var playlist = new Playlist
        {
            Id = 1, UserId = 1,
            PlaylistSongs = new List<PlaylistSong> { ps }
        };
        _playlistRepo.Setup(r => r.GetWithSongsAsync(1)).ReturnsAsync(playlist);

        await _sut.RemoveSongAsync(1, 1, 10);

        Assert.Empty(playlist.PlaylistSongs);
        _playlistRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
