using Moq;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Application.Services;
using MusicForU.Domain.Entities;

namespace MusicForU.Tests;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User>> _usersRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_usersRepo.Object, _tokenService.Object);
    }

    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsToken()
    {
        _usersRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());
        _tokenService.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("fake-token");

        var result = await _sut.RegisterAsync(new RegisterDto("Test", "test@test.com", "123456"));

        Assert.Equal("fake-token", result.Token);
        Assert.Equal("Test", result.Name);
        _usersRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _usersRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsException()
    {
        _usersRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { new User { Email = "test@test.com" } });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.RegisterAsync(new RegisterDto("Test", "test@test.com", "123456")));
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ReturnsNull()
    {
        _usersRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());

        var result = await _sut.LoginAsync(new LoginDto("wrong@test.com", "123456"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var user = new User
        {
            Id = 1,
            Name = "Test",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
        };
        _usersRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });
        _tokenService.Setup(t => t.GenerateToken(user)).Returns("fake-token");

        var result = await _sut.LoginAsync(new LoginDto("test@test.com", "123456"));

        Assert.NotNull(result);
        Assert.Equal("fake-token", result!.Token);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
        };
        _usersRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User> { user });

        var result = await _sut.LoginAsync(new LoginDto("test@test.com", "wrongpass"));

        Assert.Null(result);
    }
}
