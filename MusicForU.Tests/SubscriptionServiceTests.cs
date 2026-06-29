using Moq;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Application.Services;
using MusicForU.Domain.Entities;

namespace MusicForU.Tests;

public class SubscriptionServiceTests
{
    private readonly Mock<IRepository<Subscription>> _subRepo = new();
    private readonly Mock<IRepository<Plan>> _planRepo = new();
    private readonly SubscriptionService _sut;

    public SubscriptionServiceTests()
    {
        _sut = new SubscriptionService(_subRepo.Object, _planRepo.Object);
    }

    [Fact]
    public async Task SubscribeAsync_NewSubscription_CreatesWithCorrectEndDate()
    {
        var plan = new Plan { Id = 2, Name = "Premium", Price = 19.90m, DurationDays = 30 };
        _planRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(plan);
        _subRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(new List<Subscription>());

        var result = await _sut.SubscribeAsync(new CreateSubscriptionDto(1, 2));

        Assert.Equal("Premium", result.PlanName);
        Assert.True(result.IsActive);
        Assert.Equal(result.StartDate.AddDays(30).Date, result.EndDate.Date);
        _subRepo.Verify(r => r.AddAsync(It.IsAny<Subscription>()), Times.Once);
        _subRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SubscribeAsync_ExistingSubscription_Updates()
    {
        var plan = new Plan { Id = 3, Name = "Family", Price = 34.90m, DurationDays = 30 };
        _planRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(plan);
        var existing = new Subscription { Id = 1, UserId = 1, PlanId = 1 };
        _subRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(new List<Subscription> { existing });

        var result = await _sut.SubscribeAsync(new CreateSubscriptionDto(1, 3));

        Assert.Equal("Family", result.PlanName);
        _subRepo.Verify(r => r.Update(existing), Times.Once);
        _subRepo.Verify(r => r.AddAsync(It.IsAny<Subscription>()), Times.Never);
    }

    [Fact]
    public async Task SubscribeAsync_InvalidPlan_ThrowsException()
    {
        _planRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Plan?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.SubscribeAsync(new CreateSubscriptionDto(1, 99)));
    }

    [Fact]
    public async Task GetByUserIdAsync_NoSubscription_ReturnsNull()
    {
        _subRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(new List<Subscription>());

        var result = await _sut.GetByUserIdAsync(1);

        Assert.Null(result);
    }
}
