using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;

namespace MusicForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;
    public SubscriptionController(ISubscriptionService service) => _service = service;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Subscribe(CreateSubscriptionDto dto)
    {
        try { return Ok(await _service.SubscribeAsync(dto)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var result = await _service.GetByUserIdAsync(UserId);
        return result is null ? NotFound(new { error = "Sem assinatura ativa." }) : Ok(result);
    }
}
