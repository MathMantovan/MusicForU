using Moq;
using MusicForU.Application.Interfaces;
using MusicForU.Application.Services;
using MusicForU.Domain.Entities;

namespace MusicForU.Tests;

public class FavoriteServiceTests
{
    private readonly Mock<IRepository<Favorite>> _favRepo = new();
    private readonly Mock<ISongRepository> _songRepo = new();
    private readonly FavoriteService _sut;

    public FavoriteServiceTests()
    {
        _sut = new FavoriteService(_favRepo.Object, _songRepo.Object);
    }

    [Fact]
    public async Task FavoriteSongAsync_NewFavorite_AddsAndSaves()
    {
        _favRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Favorite, bool>>>()))
            .ReturnsAsync(new List<Favorite>());

        await _sut.FavoriteSongAsync(1, 10);

        _favRepo.Verify(r => r.AddAsync(It.Is<Favorite>(f => f.UserId == 1 && f.SongId == 10)), Times.Once);
        _favRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task FavoriteSongAsync_AlreadyExists_DoesNotDuplicate()
    {
        _favRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Favorite, bool>>>()))
            .ReturnsAsync(new List<Favorite> { new Favorite { UserId = 1, SongId = 10 } });

        await _sut.FavoriteSongAsync(1, 10);

        _favRepo.Verify(r => r.AddAsync(It.IsAny<Favorite>()), Times.Never);
    }

    [Fact]
    public async Task RemoveFavoriteSongAsync_Exists_RemovesAndSaves()
    {
        var fav = new Favorite { UserId = 1, SongId = 10 };
        _favRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Favorite, bool>>>()))
            .ReturnsAsync(new List<Favorite> { fav });

        await _sut.RemoveFavoriteSongAsync(1, 10);

        _favRepo.Verify(r => r.Remove(fav), Times.Once);
        _favRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveFavoriteSongAsync_NotExists_DoesNothing()
    {
        _favRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Favorite, bool>>>()))
            .ReturnsAsync(new List<Favorite>());

        await _sut.RemoveFavoriteSongAsync(1, 10);

        _favRepo.Verify(r => r.Remove(It.IsAny<Favorite>()), Times.Never);
    }

    [Fact]
    public async Task GetFavoritesAsync_ReturnsSongDetails()
    {
        var favs = new List<Favorite> { new Favorite { UserId = 1, SongId = 5 } };
        _favRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Favorite, bool>>>()))
            .ReturnsAsync(favs);

        var band = new Band { Name = "Nirvana" };
        var album = new Album { Title = "Nevermind", Band = band };
        var song = new Song { Id = 5, Title = "Smells Like Teen Spirit", Album = album };
        _songRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new List<Song> { song });

        var result = await _sut.GetFavoritesAsync(1);

        Assert.Single(result.Songs);
        Assert.Equal("Smells Like Teen Spirit", result.Songs[0].Title);
        Assert.Equal("Nevermind", result.Songs[0].Album);
        Assert.Equal("Nirvana", result.Songs[0].Band);
    }
}
