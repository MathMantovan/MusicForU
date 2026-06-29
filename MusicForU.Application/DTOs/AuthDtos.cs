namespace MusicForU.Application.DTOs;

public record RegisterDto(string Name, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResultDto(int UserId, string Token, string Name, string Email);
