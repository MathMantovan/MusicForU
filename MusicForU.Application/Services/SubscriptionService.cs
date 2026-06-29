using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Domain.Entities;

namespace MusicForU.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IRepository<Subscription> _subscriptions;
    private readonly IRepository<Plan> _plans;

    public SubscriptionService(IRepository<Subscription> subscriptions, IRepository<Plan> plans)
    {
        _subscriptions = subscriptions;
        _plans = plans;
    }

    public async Task<SubscriptionResultDto> SubscribeAsync(CreateSubscriptionDto dto)
    {
        var plan = await _plans.GetByIdAsync(dto.PlanId)
            ?? throw new InvalidOperationException("Plano inexistente.");

        var existing = (await _subscriptions.FindAsync(s => s.UserId == dto.UserId)).FirstOrDefault();
        if (existing is not null)
        {
            existing.PlanId = dto.PlanId;
            existing.StartDate = DateTime.UtcNow;
            existing.EndDate = DateTime.UtcNow.AddDays(plan.DurationDays);
            _subscriptions.Update(existing);
            await _subscriptions.SaveChangesAsync();
            return new SubscriptionResultDto(existing.Id, plan.Name, existing.StartDate, existing.EndDate, existing.IsActive);
        }

        var sub = new Subscription
        {
            UserId = dto.UserId,
            PlanId = dto.PlanId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(plan.DurationDays)
        };
        await _subscriptions.AddAsync(sub);
        await _subscriptions.SaveChangesAsync();
        return new SubscriptionResultDto(sub.Id, plan.Name, sub.StartDate, sub.EndDate, sub.IsActive);
    }

    public async Task<SubscriptionResultDto?> GetByUserIdAsync(int userId)
    {
        var sub = (await _subscriptions.FindAsync(s => s.UserId == userId)).FirstOrDefault();
        if (sub is null) return null;

        var plan = await _plans.GetByIdAsync(sub.PlanId);
        return new SubscriptionResultDto(sub.Id, plan!.Name, sub.StartDate, sub.EndDate, sub.IsActive);
    }
}
