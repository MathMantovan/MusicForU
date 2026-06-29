using MusicForU.Domain.Entities;

namespace MusicForU.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
