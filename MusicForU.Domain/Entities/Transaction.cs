namespace MusicForU.Domain.Entities;

public enum TransactionStatus { Authorized, Declined }

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string MerchantId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TransactionStatus Status { get; set; }
}
