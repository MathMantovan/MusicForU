using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;
using MusicForU.Domain.Entities;

namespace MusicForU.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _users;
    private readonly ITokenService _tokens;

    public AuthService(IRepository<User> users, ITokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
    {
        var exists = await _users.FindAsync(u => u.Email == dto.Email);
        if (exists.Any())
            throw new InvalidOperationException("E-mail já cadastrado.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        var token = _tokens.GenerateToken(user);
        return new AuthResultDto(user.Id, token, user.Name, user.Email);
    }

    public async Task<AuthResultDto?> LoginAsync(LoginDto dto)
    {
        var user = (await _users.FindAsync(u => u.Email == dto.Email)).FirstOrDefault();
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var token = _tokens.GenerateToken(user);
        return new AuthResultDto(user.Id, token, user.Name, user.Email);
    }
}
