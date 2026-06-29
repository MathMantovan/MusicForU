using MusicForU.Application.Interfaces;

namespace MusicForU.Infrastructure.Notifications;

public class ConsoleNotificationService : INotificationService
{
    public Task NotifyMerchantAsync(string merchantId, decimal amount)
    {
        Console.WriteLine($"[Comerciante {merchantId}] Transação de {amount:C} autorizada.");
        return Task.CompletedTask;
    }

    public Task NotifyCardHolderAsync(string email, decimal amount)
    {
        Console.WriteLine($"[Cliente {email}] Sua transação de {amount:C} foi autorizada.");
        return Task.CompletedTask;
    }
}
