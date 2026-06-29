namespace MusicForU.Application.DTOs;

public record FavoriteSongDto(int SongId, string Title, string Album, string Band);
public record FavoritesResultDto(List<FavoriteSongDto> Songs, List<int> Bands);
