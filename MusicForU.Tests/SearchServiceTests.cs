using Moq;
using MusicForU.Application.Interfaces;
using MusicForU.Application.Services;
using MusicForU.Domain.Entities;

namespace MusicForU.Tests;

public class SearchServiceTests
{
    private readonly Mock<ISongRepository> _songRepo = new();
    private readonly SearchService _sut;

    public SearchServiceTests()
    {
        _sut = new SearchService(_songRepo.Object);
    }

    [Fact]
    public async Task SearchSongsAsync_EmptyTerm_ReturnsEmpty()
    {
        var result = await _sut.SearchSongsAsync("", 1, 20);

        Assert.Empty(result);
        _songRepo.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SearchSongsAsync_ValidTerm_ReturnsMappedDtos()
    {
        var band = new Band { Id = 1, Name = "Queen" };
        var album = new Album { Id = 1, Title = "A Night at the Opera", Band = band };
        var songs = new List<Song>
        {
            new Song { Id = 1, Title = "Bohemian Rhapsody", Album = album }
        };
        _songRepo.Setup(r => r.SearchAsync("Bohemian", 0, 20)).ReturnsAsync(songs);

        var result = (await _sut.SearchSongsAsync("Bohemian", 1, 20)).ToList();

        Assert.Single(result);
        Assert.Equal("Bohemian Rhapsody", result[0].Title);
        Assert.Equal("A Night at the Opera", result[0].Album);
        Assert.Equal("Queen", result[0].Band);
    }

    [Fact]
    public async Task SearchSongsAsync_PageSizeClamped()
    {
        _songRepo.Setup(r => r.SearchAsync("test", 0, 50)).ReturnsAsync(new List<Song>());

        await _sut.SearchSongsAsync("test", 1, 100);

        _songRepo.Verify(r => r.SearchAsync("test", 0, 50), Times.Once);
    }
}
