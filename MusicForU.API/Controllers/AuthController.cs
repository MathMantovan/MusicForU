using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;

namespace MusicForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try { return Ok(await _auth.RegisterAsync(dto)); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        return result is null
            ? Unauthorized(new { error = "Credenciais inválidas." })
            : Ok(result);
    }
}
