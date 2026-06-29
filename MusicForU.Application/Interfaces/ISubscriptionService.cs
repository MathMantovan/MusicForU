using MusicForU.Application.DTOs;

namespace MusicForU.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionResultDto> SubscribeAsync(CreateSubscriptionDto dto);
    Task<SubscriptionResultDto?> GetByUserIdAsync(int userId);
}
