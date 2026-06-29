using MusicForU.Application.DTOs;

namespace MusicForU.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionResultDto> AuthorizeAsync(AuthorizeTransactionDto dto);
}
