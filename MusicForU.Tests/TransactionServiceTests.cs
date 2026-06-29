using Moq;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Application.Services;
using MusicForU.Domain.Entities;

namespace MusicForU.Tests;

public class TransactionServiceTests
{
    private readonly Mock<IRepository<User>> _usersRepo = new();
    private readonly Mock<IRepository<Transaction>> _txRepo = new();
    private readonly Mock<INotificationService> _notifications = new();
    private readonly TransactionService _sut;

    public TransactionServiceTests()
    {
        _sut = new TransactionService(_usersRepo.Object, _txRepo.Object, _notifications.Object);
    }

    [Fact]
    public async Task AuthorizeAsync_ValidTransaction_ReturnsAuthorized()
    {
        var user = new User { Id = 1, Email = "user@test.com" };
        _usersRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _txRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, bool>>>()))
            .ReturnsAsync(new List<Transaction>());

        var result = await _sut.AuthorizeAsync(new AuthorizeTransactionDto(1, "merchant1", 50m));

        Assert.True(result.Authorized);
        Assert.Equal("Transação autorizada.", result.Message);
        _txRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _notifications.Verify(n => n.NotifyMerchantAsync("merchant1", 50m), Times.Once);
        _notifications.Verify(n => n.NotifyCardHolderAsync("user@test.com", 50m), Times.Once);
    }

    [Fact]
    public async Task AuthorizeAsync_UserNotFound_ReturnsFalse()
    {
        _usersRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _sut.AuthorizeAsync(new AuthorizeTransactionDto(99, "merchant1", 50m));

        Assert.False(result.Authorized);
        Assert.Equal("Usuário inexistente.", result.Message);
    }

    [Fact]
    public async Task AuthorizeAsync_InvalidAmount_ReturnsFalse()
    {
        _usersRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1 });

        var result = await _sut.AuthorizeAsync(new AuthorizeTransactionDto(1, "merchant1", 0m));

        Assert.False(result.Authorized);
        Assert.Equal("Valor inválido.", result.Message);
    }

    [Fact]
    public async Task AuthorizeAsync_TooSoon_ReturnsFalse()
    {
        _usersRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1 });
        var recentTx = new Transaction { UserId = 1, CreatedAt = DateTime.UtcNow.AddSeconds(-10) };
        _txRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, bool>>>()))
            .ReturnsAsync(new List<Transaction> { recentTx });

        var result = await _sut.AuthorizeAsync(new AuthorizeTransactionDto(1, "merchant1", 50m));

        Assert.False(result.Authorized);
        Assert.Equal("Transação muito próxima da anterior.", result.Message);
    }
}
