namespace MusicForU.Application.DTOs;

public record AuthorizeTransactionDto(int UserId, string MerchantId, decimal Amount);
public record TransactionResultDto(bool Authorized, string Message);
