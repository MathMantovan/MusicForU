using MusicForU.Application.DTOs;

namespace MusicForU.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto dto);
    Task<AuthResultDto?> LoginAsync(LoginDto dto);
}
