namespace MusicForU.Application.Interfaces;

public interface INotificationService
{
    Task NotifyMerchantAsync(string merchantId, decimal amount);
    Task NotifyCardHolderAsync(string email, decimal amount);
}
