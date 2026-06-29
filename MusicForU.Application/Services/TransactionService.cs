using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Domain.Entities;

namespace MusicForU.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<Transaction> _transactions;
    private readonly INotificationService _notifications;

    private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(30);

    public TransactionService(
        IRepository<User> users,
        IRepository<Transaction> transactions,
        INotificationService notifications)
    {
        _users = users;
        _transactions = transactions;
        _notifications = notifications;
    }

    public async Task<TransactionResultDto> AuthorizeAsync(AuthorizeTransactionDto dto)
    {
        var user = await _users.GetByIdAsync(dto.UserId);
        if (user is null)
            return new TransactionResultDto(false, "Usuário inexistente.");

        if (dto.Amount <= 0)
            return new TransactionResultDto(false, "Valor inválido.");

        var last = (await _transactions.FindAsync(t => t.UserId == dto.UserId))
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefault();

        if (last is not null && DateTime.UtcNow - last.CreatedAt < MinInterval)
            return new TransactionResultDto(false, "Transação muito próxima da anterior.");

        var tx = new Transaction
        {
            UserId = dto.UserId,
            MerchantId = dto.MerchantId,
            Amount = dto.Amount,
            Status = TransactionStatus.Authorized
        };
        await _transactions.AddAsync(tx);
        await _transactions.SaveChangesAsync();

        await _notifications.NotifyMerchantAsync(dto.MerchantId, dto.Amount);
        await _notifications.NotifyCardHolderAsync(user.Email, dto.Amount);

        return new TransactionResultDto(true, "Transação autorizada.");
    }
}
