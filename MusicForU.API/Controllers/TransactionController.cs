using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;

namespace MusicForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _service;
    public TransactionController(ITransactionService service) => _service = service;

    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize(AuthorizeTransactionDto dto)
        => Ok(await _service.AuthorizeAsync(dto));
}
